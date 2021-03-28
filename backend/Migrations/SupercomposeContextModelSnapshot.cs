﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using supercompose;

namespace backend2.Migrations
{
    [DbContext(typeof(SupercomposeContext))]
    partial class SupercomposeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresEnum(null, "connection_log_severity", new[] { "info", "error", "warning" })
                .HasPostgresExtension("uuid-ossp")
                .HasAnnotation("Relational:Collation", "en_US.UTF-8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("backend2.Context.ConnectionLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ComposeId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("DeploymentId")
                        .HasColumnType("uuid");

                    b.Property<string>("Error")
                        .HasColumnType("text");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Dictionary<string, object>>("Metadata")
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("NodeId")
                        .HasColumnType("uuid");

                    b.Property<int>("Severity")
                        .HasColumnType("integer");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("ComposeId");

                    b.HasIndex("DeploymentId");

                    b.HasIndex("NodeId");

                    b.HasIndex("TenantId");

                    b.ToTable("ConnectionLogs");
                });

            modelBuilder.Entity("supercompose.Compose", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CurrentId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CurrentId")
                        .IsUnique();

                    b.HasIndex("TenantId");

                    b.ToTable("Composes");
                });

            modelBuilder.Entity("supercompose.ComposeVersion", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ComposeId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Directory")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("PendingDelete")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("RedeploymentRequestedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool?>("ServiceEnabled")
                        .IsRequired()
                        .HasColumnType("boolean");

                    b.Property<string>("ServiceName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Id");

                    b.HasIndex("ComposeId");

                    b.ToTable("ComposeVersions");
                });

            modelBuilder.Entity("supercompose.Deployment", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ComposeId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastCheck")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool?>("LastDeployedAsEnabled")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("LastDeployedComposeVersionId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("LastDeployedNodeVersion")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("NodeId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<bool?>("ReconciliationFailed")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("RedeploymentRequestedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("LastDeployedComposeVersionId");

                    b.HasIndex("NodeId");

                    b.HasIndex("ComposeId", "NodeId")
                        .IsUnique();

                    b.ToTable("Deployments");
                });

            modelBuilder.Entity("supercompose.Node", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Host")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<byte[]>("Password")
                        .HasColumnType("bytea");

                    b.Property<int?>("Port")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<byte[]>("PrivateKey")
                        .HasColumnType("bytea");

                    b.Property<bool?>("ReconciliationFailed")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("RedeploymentRequestedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uuid");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("Version")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Nodes");
                });

            modelBuilder.Entity("supercompose.Tenant", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Tenants");
                });

            modelBuilder.Entity("backend2.Context.ConnectionLog", b =>
                {
                    b.HasOne("supercompose.Compose", "Compose")
                        .WithMany("ConnectionLogs")
                        .HasForeignKey("ComposeId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("supercompose.Deployment", "Deployment")
                        .WithMany("ConnectionLogs")
                        .HasForeignKey("DeploymentId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("supercompose.Node", "Node")
                        .WithMany("ConnectionLogs")
                        .HasForeignKey("NodeId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("supercompose.Tenant", "Tenant")
                        .WithMany("ConnectionLogs")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Compose");

                    b.Navigation("Deployment");

                    b.Navigation("Node");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("supercompose.Compose", b =>
                {
                    b.HasOne("supercompose.ComposeVersion", "Current")
                        .WithOne("Compose")
                        .HasForeignKey("supercompose.Compose", "CurrentId")
                        .IsRequired();

                    b.HasOne("supercompose.Tenant", "Tenant")
                        .WithMany("Composes")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Current");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("supercompose.ComposeVersion", b =>
                {
                    b.HasOne("supercompose.Compose", "ComposeNavigation")
                        .WithMany("ComposeVersions")
                        .HasForeignKey("ComposeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ComposeNavigation");
                });

            modelBuilder.Entity("supercompose.Deployment", b =>
                {
                    b.HasOne("supercompose.Compose", "Compose")
                        .WithMany("Deployments")
                        .HasForeignKey("ComposeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("supercompose.ComposeVersion", "LastDeployedComposeVersion")
                        .WithMany("Deployments")
                        .HasForeignKey("LastDeployedComposeVersionId");

                    b.HasOne("supercompose.Node", "Node")
                        .WithMany("Deployments")
                        .HasForeignKey("NodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Compose");

                    b.Navigation("LastDeployedComposeVersion");

                    b.Navigation("Node");
                });

            modelBuilder.Entity("supercompose.Node", b =>
                {
                    b.HasOne("supercompose.Tenant", "Tenant")
                        .WithMany("Nodes")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("supercompose.Compose", b =>
                {
                    b.Navigation("ComposeVersions");

                    b.Navigation("ConnectionLogs");

                    b.Navigation("Deployments");
                });

            modelBuilder.Entity("supercompose.ComposeVersion", b =>
                {
                    b.Navigation("Compose");

                    b.Navigation("Deployments");
                });

            modelBuilder.Entity("supercompose.Deployment", b =>
                {
                    b.Navigation("ConnectionLogs");
                });

            modelBuilder.Entity("supercompose.Node", b =>
                {
                    b.Navigation("ConnectionLogs");

                    b.Navigation("Deployments");
                });

            modelBuilder.Entity("supercompose.Tenant", b =>
                {
                    b.Navigation("Composes");

                    b.Navigation("ConnectionLogs");

                    b.Navigation("Nodes");
                });
#pragma warning restore 612, 618
        }
    }
}
