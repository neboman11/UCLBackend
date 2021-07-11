﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UCLBackend.DataAccess.Models;

namespace UCLBackend.Service.Migrations
{
    [DbContext(typeof(UCLContext))]
    [Migration("20210711061503_AddLeagueRoleIDs")]
    partial class AddLeagueRoleIDs
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("UCLBackend.DataAccess.Models.Account", b =>
                {
                    b.Property<string>("AccountID")
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("IsPrimary")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Platform")
                        .HasColumnType("longtext");

                    b.Property<string>("PlayerID")
                        .HasColumnType("varchar(255)");

                    b.HasKey("AccountID");

                    b.HasIndex("PlayerID");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("UCLBackend.DataAccess.Models.Player", b =>
                {
                    b.Property<string>("PlayerID")
                        .HasColumnType("varchar(255)");

                    b.Property<int?>("CurrentMMR")
                        .HasColumnType("int");

                    b.Property<ulong>("DiscordID")
                        .HasColumnType("bigint unsigned");

                    b.Property<bool?>("IsFreeAgent")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<int?>("PeakMMR")
                        .HasColumnType("int");

                    b.Property<double?>("Salary")
                        .HasColumnType("double");

                    b.Property<int?>("TeamID")
                        .HasColumnType("int");

                    b.HasKey("PlayerID");

                    b.HasIndex("TeamID");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("UCLBackend.DataAccess.Models.Setting", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("Key");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("UCLBackend.DataAccess.Models.Team", b =>
                {
                    b.Property<int>("TeamID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<double>("CapSpace")
                        .HasColumnType("double");

                    b.Property<string>("Conference")
                        .HasColumnType("longtext");

                    b.Property<string>("League")
                        .HasColumnType("longtext");

                    b.Property<string>("TeamName")
                        .HasColumnType("longtext");

                    b.HasKey("TeamID");

                    b.ToTable("Roster");
                });

            modelBuilder.Entity("UCLBackend.DataAccess.Models.Account", b =>
                {
                    b.HasOne("UCLBackend.DataAccess.Models.Player", "Player")
                        .WithMany("Accounts")
                        .HasForeignKey("PlayerID");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("UCLBackend.DataAccess.Models.Player", b =>
                {
                    b.HasOne("UCLBackend.DataAccess.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamID");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("UCLBackend.DataAccess.Models.Player", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
