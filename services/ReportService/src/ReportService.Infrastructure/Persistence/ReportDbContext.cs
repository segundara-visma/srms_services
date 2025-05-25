using Microsoft.EntityFrameworkCore;
using ReportService.Domain.Entities;
using ReportService.Application.DTOs;

namespace ReportService.Infrastructure.Persistence;

public class ReportDbContext : DbContext
{
    public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options) { }

    public DbSet<Report> Reports { get; set; }
    public DbSet<ReportDetail> ReportDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Report>().HasKey(r => r.Id);
        modelBuilder.Entity<ReportDetail>().HasKey(rd => rd.Id);
        modelBuilder.Entity<ReportDetail>()
            .HasOne<Report>()
            .WithMany(r => r.ReportDetails)
            .HasForeignKey(rd => rd.ReportId);

        // Seed data with current date and time (09:58 PM EEST, May 24, 2025)
        var generatedAt = new DateTime(2025, 5, 24, 18, 58, 0, DateTimeKind.Utc); // 09:58 PM EEST (UTC+3)
        var reportId = new Guid("c6afdaf1-93f9-4a43-a743-4c762667c15f");
        modelBuilder.Entity<Report>().HasData(
            new Report
            {
                Id = reportId,
                StudentId = new Guid("b1c2d3e4-f5f6-7890-abcd-ef1234567890"),
                GeneratedAt = generatedAt,
                GPA = 3.7m,
                Status = "Completed"
            }
        );
        modelBuilder.Entity<ReportDetail>().HasData(
            new ReportDetail
            {
                Id = new Guid("0f0ae5cf-ebe6-4440-ac49-0872244a0b33"),
                ReportId = reportId,
                CourseId = new Guid("0365eed4-e67d-460a-abd7-6742b3698d86"),
                Grade = 90.0m,
                CourseTitle = "Introduction to Physics",
                Credits = 3,
                Status = "Completed"
            }
        );
    }
}