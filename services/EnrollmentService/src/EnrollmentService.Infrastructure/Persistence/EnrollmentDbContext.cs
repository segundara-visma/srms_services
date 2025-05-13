using Microsoft.EntityFrameworkCore;
using EnrollmentService.Domain.Entities;
using System;

namespace EnrollmentService.Infrastructure.Persistence;

public class EnrollmentDbContext : DbContext
{
    public EnrollmentDbContext(DbContextOptions<EnrollmentDbContext> options) : base(options) { }

    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the primary key
        modelBuilder.Entity<Enrollment>().HasKey(e => e.Id);

        // Configure the Status property with proper conversion
        modelBuilder.Entity<Enrollment>()
            .Property(e => e.Status)
            .HasConversion(
                status => status.ToString(), // Convert EnrollmentStatus to string for the database
                dbValue => ParseEnrollmentStatus(dbValue) // Convert string back to EnrollmentStatus
            );

        // Seed data with current date and time (11:58 AM EEST, May 13, 2025)
        var enrollmentDate = new DateTime(2025, 5, 13, 11, 58, 0, DateTimeKind.Utc); //.ToLocalTime(); // Convert to EEST (UTC+3)
        modelBuilder.Entity<Enrollment>().HasData(
            new Enrollment
            {
                Id = new Guid("4c7890df-40ca-48ab-88d2-ff510fd4f818"),
                StudentId = new Guid("b1c2d3e4-f5f6-7890-abcd-ef1234567890"), // Note this is userId(user.id) from user service and not studentId(student.id) from student service.
                CourseId = new Guid("0365eed4-e67d-460a-abd7-6742b3698d86"),
                EnrollmentDate = enrollmentDate,
                Status = EnrollmentStatus.Enrolled
            }
        );
    }

    // Helper method to parse EnrollmentStatus outside the expression tree
    private static EnrollmentStatus ParseEnrollmentStatus(string dbValue)
    {
        return Enum.TryParse<EnrollmentStatus>(dbValue, out var result) ? result : EnrollmentStatus.Enrolled;
    }
}