﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ReportService.Infrastructure.Persistence;

#nullable disable

namespace ReportService.Infrastructure.Migrations
{
    [DbContext(typeof(ReportDbContext))]
    [Migration("20250525082715_reportServiceMigration")]
    partial class reportServiceMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ReportService.Domain.Entities.Report", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal?>("GPA")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("GeneratedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Reports");

                    b.HasData(
                        new
                        {
                            Id = new Guid("c6afdaf1-93f9-4a43-a743-4c762667c15f"),
                            GPA = 3.7m,
                            GeneratedAt = new DateTime(2025, 5, 24, 18, 58, 0, 0, DateTimeKind.Utc),
                            Status = "Completed",
                            StudentId = new Guid("b1c2d3e4-f5f6-7890-abcd-ef1234567890")
                        });
                });

            modelBuilder.Entity("ReportService.Domain.Entities.ReportDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid");

                    b.Property<string>("CourseTitle")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Credits")
                        .HasColumnType("integer");

                    b.Property<decimal?>("Grade")
                        .HasColumnType("numeric");

                    b.Property<Guid>("ReportId")
                        .HasColumnType("uuid");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ReportId");

                    b.ToTable("ReportDetails");

                    b.HasData(
                        new
                        {
                            Id = new Guid("0f0ae5cf-ebe6-4440-ac49-0872244a0b33"),
                            CourseId = new Guid("0365eed4-e67d-460a-abd7-6742b3698d86"),
                            CourseTitle = "Introduction to Physics",
                            Credits = 3,
                            Grade = 90.0m,
                            ReportId = new Guid("c6afdaf1-93f9-4a43-a743-4c762667c15f"),
                            Status = "Completed"
                        });
                });

            modelBuilder.Entity("ReportService.Domain.Entities.ReportDetail", b =>
                {
                    b.HasOne("ReportService.Domain.Entities.Report", null)
                        .WithMany("ReportDetails")
                        .HasForeignKey("ReportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ReportService.Domain.Entities.Report", b =>
                {
                    b.Navigation("ReportDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
