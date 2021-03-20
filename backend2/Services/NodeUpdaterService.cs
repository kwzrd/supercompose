using backend2.HostedServices;
using backend2.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using StackExchange.Redis;
using supercompose;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using backend2.Exceptions;
using Renci.SshNet.Common;

namespace backend2.Services
{
  /// <summary>
  /// This is a core component that performs pending updates for individual nodes
  /// </summary>
  public class NodeUpdaterService
  {
    public static readonly TimeSpan NodeCheckInterval = TimeSpan.FromHours(1);

    private readonly ILogger<NodeUpdaterService> logger;
    private readonly SupercomposeContext ctx;
    private readonly IConnectionMultiplexer multiplexer;
    private readonly ConnectionService connectionService;
    private readonly CryptoService cryptoService;
    private readonly ConnectionLogService connectionLog;

    public NodeUpdaterService(
      ILogger<NodeUpdaterService> logger,
      SupercomposeContext ctx,
      IConnectionMultiplexer multiplexer,
      ConnectionService connectionService,
      CryptoService cryptoService,
      ConnectionLogService connectionLog
    )
    {
      this.logger = logger;
      this.ctx = ctx;
      this.multiplexer = multiplexer;
      this.connectionService = connectionService;
      this.cryptoService = cryptoService;
      this.connectionLog = connectionLog;
    }

    public async Task NotifyAboutNodeChange(Guid nodeId)
    {
      await multiplexer.GetSubscriber().PublishAsync(NodeUpdateListener.ChannelName, nodeId.ToString());
    }

    public async Task ProcessNodeUpdates(Guid nodeId, CancellationToken ct)
    {
      try
      {
        while (!ct.IsCancellationRequested)
        {
          var deployments = await ctx.Deployments
            .Where(x => x.NodeId == nodeId)
            .Where(Deployment.ShouldUpdateProjection)
            .Include(x => x.Node)
            .Include(x => x.LastDeployedComposeVersion)
            .Include(x => x.Compose)
            .ThenInclude(x => x.Current)
            .ToListAsync(ct);

          if (deployments.Count == 0) return;

          var node = deployments.First().Node;
          using var ssh = await OpenSsh(node, ct);

          if (deployments.Any(x => x.LastDeployedNodeVersion != node.Version))
            if (!await VerifyNode(node, ssh, ct))
              return;

          using var sftp = await OpenSftp(node, ct);

          foreach (var deployment in deployments) await ApplyDeployment(deployment, ssh, sftp, ct);
        }
      }
      catch (TaskCanceledException)
      {
      }
      catch (NodeConnectionFailedException ex)
      {
        connectionLog.Error($"Node connection failed", ex);
      }
      catch (SshException ex)
      {
        connectionLog.Error($"SSH error", ex);
      }
      catch (Exception ex)
      {
        connectionLog.Error($"Unknown error", ex);
      }
    }

    private async Task<SshClient> OpenSsh(Node node, CancellationToken ct)
    {
      logger.LogDebug("Pending changes detected, opening ssh connection");

      var connectionParams = new ConnectionParams
      (
        node.Host,
        node.Username,
        node.Port!.Value,
        node.Password == null ? null : await cryptoService.DecryptSecret(node.Password),
        node.PrivateKey == null ? null : await cryptoService.DecryptSecret(node.PrivateKey)
      );

      connectionLog.Info($"Connecting SSH to {node.Username}@{node.Host}:{node.Port}");
      var ssh = await connectionService.CreateSshConnection(connectionParams, TimeSpan.FromSeconds(10), ct);

      ct.ThrowIfCancellationRequested();

      return ssh;
    }

    private async Task<SftpClient> OpenSftp(Node node, CancellationToken ct)
    {
      logger.LogDebug("Pending changes detected, opening sftp connection");

      var connectionParams = new ConnectionParams
      (
        node.Host,
        node.Username,
        node.Port!.Value,
        node.Password == null ? null : await cryptoService.DecryptSecret(node.Password),
        node.PrivateKey == null ? null : await cryptoService.DecryptSecret(node.PrivateKey)
      );

      connectionLog.Info($"Connecting SFTP to {node.Username}@{node.Host}:{node.Port}");
      var sftp = await connectionService.CreateSftpConnection(connectionParams, TimeSpan.FromSeconds(10), ct);

      ct.ThrowIfCancellationRequested();

      return sftp;
    }

    private async Task<(string result, string error, int status)> RunCommand(SshClient ssh, string command,
      CancellationToken ct)
    {
      connectionLog.Info($"Running command: {command}");

      var result = await connectionService.RunCommand(ssh, command, TimeSpan.FromSeconds(10), ct);

      if (result.status != 0) connectionLog.Info($"Command failed with status: {result.status}");

      return result;
    }

    private async Task<bool> VerifyNode(Node node, SshClient ssh, CancellationToken ct)
    {
      connectionLog.Info("Enabling node");

      var systemctlVersion = await RunCommand(ssh, "systemctl --version", ct);
      if (systemctlVersion.status != 0)
      {
        logger.LogDebug("systemd unavailable");
        connectionLog.Error("systemd unavailable, stopping node configuration");
        node.ReconciliationFailed = true;
        await ctx.SaveChangesAsync();
        return false;
      }

      var dockerVersion = await RunCommand(ssh, "docker --version", ct);
      if (dockerVersion.status != 0)
      {
        logger.LogDebug("docker unavailable");
        connectionLog.Error("docker unavailable, stopping node configuration");
        node.ReconciliationFailed = true;
        await ctx.SaveChangesAsync();
        return false;
      }

      var dockerComposeVersion = await RunCommand(ssh, "docker-compose --version", ct);
      if (dockerComposeVersion.status != 0)
      {
        logger.LogDebug("docker-compose unavailable");
        connectionLog.Error("docker-compose unavailable, stopping node configuration");
        node.ReconciliationFailed = true;
        await ctx.SaveChangesAsync();
        return false;
      }

      return true;
    }


    private async Task UpdateFile(SftpClient sftp, string path, string contents, CancellationToken ct)
    {
      connectionLog.Info($"Reading file {path}");
      var targetBytes = Encoding.UTF8.GetBytes(contents.Replace("\r\n", "\n"));

      var shouldWrite = true;
      try
      {
        var current = await connectionService.ReadFile(sftp, path, ct);
        shouldWrite = !current.SequenceEqual(targetBytes);
      }
      catch (SftpPathNotFoundException)
      {
      }

      if (shouldWrite)
      {
        connectionLog.Info($"File outdated, updating {path}");
        await connectionService.WriteFile(sftp, path, targetBytes);
      }
    }

    private async Task ApplyDeployment(Deployment deployment, SshClient ssh, SftpClient sftp, CancellationToken ct)
    {
      // TODO cleanup current version
      connectionLog.BeginScope(deploymentId: deployment.Id);
      await EnsureComposeAndServiceDeployment(deployment, ssh, sftp, ct);
      await EnsureServiceInCorrectState(deployment, ssh, ct);
    }

    private async Task EnsureComposeAndServiceDeployment(Deployment deployment, SshClient ssh, SftpClient sftp,
      CancellationToken ct)
    {
      var current = deployment.Compose.Current;

      try
      {
        connectionLog.Info($"Ensuring directory exists {current.Directory}");
        await connectionService.EnsureDirectoryExists(sftp, current.Directory, ct);

        connectionLog.Info($"Updating docker-compose.yml");
        await UpdateFile(sftp, current.ComposePath, current.Content, ct);
        ct.ThrowIfCancellationRequested();
      }
      catch (SftpPermissionDeniedException ex)
      {
        connectionLog.Error($"Docker compose synchronization failed due to permission error:", ex);
        deployment.ReconciliationFailed = true;
        await ctx.SaveChangesAsync(ct);
      }

      if (current.ServiceEnabled == true)
        try
        {
          var serviceFile = GenerateSystemdServiceFile(deployment);
          connectionLog.Info($"Updating systemd service");
          await UpdateFile(sftp, current.ServicePath, serviceFile, ct);
          ct.ThrowIfCancellationRequested();

          var reloadResult = await RunCommand(ssh, "systemctl daemon-reload", ct);

          if (reloadResult.status != 0)
            connectionLog.Error($"Systemd service reload failed with code {reloadResult.status}");
        }
        catch (SftpPermissionDeniedException ex)
        {
          connectionLog.Error($"Service synchronization failed due to permission error:", ex);
          deployment.ReconciliationFailed = true;
          await ctx.SaveChangesAsync(ct);
        }
    }

    private async Task EnsureServiceInCorrectState(Deployment deployment, SshClient ssh, CancellationToken ct)
    {
      var last = deployment.LastDeployedComposeVersion;
      var current = deployment.Compose.Current;

      var lastSvc = last?.ServiceEnabled ?? false;
      var lastEnabled = deployment?.LastDeployedAsEnabled ?? false;

      var currentSvc = current?.ServiceEnabled ?? false;
      var currentEnabled = deployment?.Enabled ?? false;

      if (last == null)
      {
        if (currentEnabled)
        {
          if (currentSvc)
            await StartSystemdService(deployment, current, ssh, ct);
          else
            await StartDockerCompose(deployment, current, ssh, ct);
        }
      }
      else
      {
        // Stop last state when it was enabled and there is a change in service
        if (lastEnabled && lastSvc != currentSvc)
        {
          if (lastSvc)
            await StopSystemdService(deployment, last, ssh, ct);
          else
            await StopDockerCompose(deployment, current, ssh, ct);
        }

        // We don't have to worry about the old state at this point. Only redeploy if there are changes.
        if (currentSvc) await StartSystemdService(deployment, current, ssh, ct);
        else await StartDockerCompose(deployment, current, ssh, ct);
      }

      deployment.LastDeployedComposeVersion = current;
      deployment.LastDeployedAsEnabled = currentEnabled;
      deployment.LastDeployedNodeVersion = deployment.Node.Version;
      deployment.LastCheck = DateTime.UtcNow;
      await ctx.SaveChangesAsync(ct);
    }

    private async Task StopSystemdService(Deployment deployment, ComposeVersion compose, SshClient ssh,
      CancellationToken ct)
    {
      var serviceStartResult =
        await RunCommand(ssh, $"systemctl stop {compose.ServiceName}", ct);

      if (serviceStartResult.status != 0)
        throw new DeploymentReconciliationFailedException("Systemd service failed to stop");

      var serviceEnableResult =
        await RunCommand(ssh, $"systemctl disable {compose.ServiceName}", ct);

      if (serviceEnableResult.status != 0)
        throw new DeploymentReconciliationFailedException("Systemd service failed to be disabled");
    }

    private async Task StartSystemdService(Deployment deployment, ComposeVersion compose, SshClient ssh,
      CancellationToken ct)
    {
      var serviceStartResult =
        await RunCommand(ssh, $"systemctl start {compose.ServiceName}", ct);

      if (serviceStartResult.status != 0)
        throw new DeploymentReconciliationFailedException("Systemd service failed to start");

      var serviceEnableResult =
        await RunCommand(ssh, $"systemctl enable {compose.ServiceName}", ct);

      if (serviceEnableResult.status != 0)
        throw new DeploymentReconciliationFailedException("Systemd service failed to be enabled");
    }

    private async Task StopDockerCompose(Deployment deployment, ComposeVersion compose, SshClient ssh,
      CancellationToken ct)
    {
      var startCommand =
        await RunCommand(ssh, $"/usr/local/bin/docker-compose --file '{compose.ComposePath}' down", ct);

      if (startCommand.status != 0)
        throw new DeploymentReconciliationFailedException($"Docker-compose failed to stop");
    }

    private async Task StartDockerCompose(Deployment deployment, ComposeVersion compose, SshClient ssh,
      CancellationToken ct)
    {
      var startCommand =
        await RunCommand(ssh, $"/usr/local/bin/docker-compose --file '{compose.ComposePath}' up -d --remove-orphans",
          ct);

      if (startCommand.status != 0)
        throw new DeploymentReconciliationFailedException("Docker-compose failed to start");
    }

    private static string GenerateSystemdServiceFile(Deployment deployment)
    {
      return $@"
[Unit]
Description={deployment.Compose.Name} service with docker compose managed by supercompose
Requires=docker.service
After=docker.service

[Service]
Type=oneshot
RemainAfterExit=true
WorkingDirectory={deployment.Compose.Current.Directory}
ExecStart=/usr/bin/docker-compose up -d --remove-orphans
ExecStop=/usr/bin/docker-compose down

[Install]
WantedBy=multi-user.target";
    }
  }
}