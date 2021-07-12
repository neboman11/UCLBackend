﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UCLBackend.Service.DataAccess.Models;

namespace UCLBackend.Service.Migrations
{
    [DbContext(typeof(UCLContext))]
    [Migration("20210707023128_AddRosterTable")]
    partial class AddRosterTable
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

                    b.Property<string>("DiscordID")
                        .HasColumnType("longtext");

                    b.Property<string>("League")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<int?>("PeakMMR")
                        .HasColumnType("int");

                    b.Property<double?>("Salary")
                        .HasColumnType("double");

                    b.Property<string>("Team")
                        .HasColumnType("longtext");

                    b.Property<string>("TimeZone")
                        .HasColumnType("longtext");

                    b.HasKey("PlayerID");

                    b.ToTable("Players");
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

                    b.Property<string>("PlayerAPlayerID")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PlayerBPlayerID")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PlayerCPlayerID")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ReservePlayerID")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("TeamName")
                        .HasColumnType("longtext");

                    b.HasKey("TeamID");

                    b.HasIndex("PlayerAPlayerID");

                    b.HasIndex("PlayerBPlayerID");

                    b.HasIndex("PlayerCPlayerID");

                    b.HasIndex("ReservePlayerID");

                    b.ToTable("Roster");
                });

            modelBuilder.Entity("UCLBackend.DataAccess.Models.Account", b =>
                {
                    b.HasOne("UCLBackend.DataAccess.Models.Player", "Player")
                        .WithMany("Accounts")
                        .HasForeignKey("PlayerID");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("UCLBackend.DataAccess.Models.Team", b =>
                {
                    b.HasOne("UCLBackend.DataAccess.Models.Player", "PlayerA")
                        .WithMany()
                        .HasForeignKey("PlayerAPlayerID");

                    b.HasOne("UCLBackend.DataAccess.Models.Player", "PlayerB")
                        .WithMany()
                        .HasForeignKey("PlayerBPlayerID");

                    b.HasOne("UCLBackend.DataAccess.Models.Player", "PlayerC")
                        .WithMany()
                        .HasForeignKey("PlayerCPlayerID");

                    b.HasOne("UCLBackend.DataAccess.Models.Player", "Reserve")
                        .WithMany()
                        .HasForeignKey("ReservePlayerID");

                    b.Navigation("PlayerA");

                    b.Navigation("PlayerB");

                    b.Navigation("PlayerC");

                    b.Navigation("Reserve");
                });

            modelBuilder.Entity("UCLBackend.DataAccess.Models.Player", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
