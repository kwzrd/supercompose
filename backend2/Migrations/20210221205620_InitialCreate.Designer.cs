﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using supercompose;

namespace supercompose.Migrations
{
  [DbContext(typeof(SupercomposeContext))]
  [Migration("20210221205620_InitialCreate")]
  partial class InitialCreate
  {
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
      modelBuilder
          .HasPostgresExtension("uuid-ossp")
          .HasAnnotation("Relational:Collation", "en_US.UTF-8")
          .HasAnnotation("Relational:MaxIdentifierLength", 63)
          .HasAnnotation("ProductVersion", "5.0.3")
          .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

      modelBuilder.Entity("backend2.Compose", b =>
          {
            b.Property<Guid>("Id")
                      .ValueGeneratedOnAdd()
                      .HasColumnType("uuid");

            b.Property<Guid?>("CurrentId")
                      .IsRequired()
                      .HasColumnType("uuid");

            b.Property<string>("Name")
                      .IsRequired()
                      .HasMaxLength(255)
                      .HasColumnType("character varying(255)");

            b.Property<bool?>("PendingDelete")
                      .IsRequired()
                      .HasColumnType("boolean");

            b.Property<Guid?>("TenantId")
                      .HasColumnType("uuid");

            b.HasKey("Id");

            b.HasIndex("CurrentId")
                      .IsUnique();

            b.HasIndex("TenantId");

            b.ToTable("Composes");
          });

      modelBuilder.Entity("backend2.ComposeVersion", b =>
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

      modelBuilder.Entity("backend2.Deployment", b =>
          {
            b.Property<Guid?>("Id")
                      .ValueGeneratedOnAdd()
                      .HasColumnType("uuid");

            b.Property<Guid?>("ComposeId")
                      .IsRequired()
                      .HasColumnType("uuid");

            b.Property<bool?>("Enabled")
                      .IsRequired()
                      .HasColumnType("boolean");

            b.Property<Guid?>("LastDeployedComposeVersionId")
                      .IsRequired()
                      .HasColumnType("uuid");

            b.Property<Guid?>("NodeId")
                      .IsRequired()
                      .HasColumnType("uuid");

            b.HasKey("Id");

            b.HasIndex("LastDeployedComposeVersionId");

            b.HasIndex("NodeId");

            b.HasIndex("ComposeId", "NodeId")
                      .IsUnique();

            b.ToTable("Deployments");
          });

      modelBuilder.Entity("backend2.Node", b =>
          {
            b.Property<Guid>("Id")
                      .ValueGeneratedOnAdd()
                      .HasColumnType("uuid");

            b.Property<bool?>("Enabled")
                      .IsRequired()
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

            b.Property<int>("Port")
                      .HasColumnType("integer");

            b.Property<byte[]>("PrivateKey")
                      .HasColumnType("bytea");

            b.Property<Guid?>("TenantId")
                      .HasColumnType("uuid");

            b.Property<string>("Username")
                      .IsRequired()
                      .HasMaxLength(255)
                      .HasColumnType("character varying(255)");

            b.HasKey("Id");

            b.HasIndex("TenantId");

            b.ToTable("Nodes");
          });

      modelBuilder.Entity("backend2.Tenant", b =>
          {
            b.Property<Guid>("Id")
                      .ValueGeneratedOnAdd()
                      .HasColumnType("uuid");

            b.HasKey("Id");

            b.ToTable("Tenants");
          });

      modelBuilder.Entity("backend2.Compose", b =>
          {
            b.HasOne("backend2.ComposeVersion", "Current")
                      .WithOne("Compose")
                      .HasForeignKey("backend2.Compose", "CurrentId")
                      .IsRequired();

            b.HasOne("backend2.Tenant", "Tenant")
                      .WithMany("Composes")
                      .HasForeignKey("TenantId")
                      .OnDelete(DeleteBehavior.Cascade);

            b.Navigation("Current");

            b.Navigation("Tenant");
          });

      modelBuilder.Entity("backend2.ComposeVersion", b =>
          {
            b.HasOne("backend2.Compose", "ComposeNavigation")
                      .WithMany("ComposeVersions")
                      .HasForeignKey("ComposeId")
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

            b.Navigation("ComposeNavigation");
          });

      modelBuilder.Entity("backend2.Deployment", b =>
          {
            b.HasOne("backend2.Compose", "Compose")
                      .WithMany("Deployments")
                      .HasForeignKey("ComposeId")
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

            b.HasOne("backend2.ComposeVersion", "LastDeployedComposeVersion")
                      .WithMany("Deployments")
                      .HasForeignKey("LastDeployedComposeVersionId")
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

            b.HasOne("backend2.Node", "Node")
                      .WithMany("Deployments")
                      .HasForeignKey("NodeId")
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

            b.Navigation("Compose");

            b.Navigation("LastDeployedComposeVersion");

            b.Navigation("Node");
          });

      modelBuilder.Entity("backend2.Node", b =>
          {
            b.HasOne("backend2.Tenant", "Tenant")
                      .WithMany("Nodes")
                      .HasForeignKey("TenantId")
                      .OnDelete(DeleteBehavior.Cascade);

            b.Navigation("Tenant");
          });

      modelBuilder.Entity("backend2.Compose", b =>
          {
            b.Navigation("ComposeVersions");

            b.Navigation("Deployments");
          });

      modelBuilder.Entity("backend2.ComposeVersion", b =>
          {
            b.Navigation("Compose");

            b.Navigation("Deployments");
          });

      modelBuilder.Entity("backend2.Node", b =>
          {
            b.Navigation("Deployments");
          });

      modelBuilder.Entity("backend2.Tenant", b =>
          {
            b.Navigation("Composes");

            b.Navigation("Nodes");
          });
#pragma warning restore 612, 618
    }
  }
}
