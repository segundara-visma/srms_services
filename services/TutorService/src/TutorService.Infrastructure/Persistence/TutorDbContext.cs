using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Application.DTOs;

namespace TutorService.Infrastructure.Persistence;

public class TutorDbContext : DbContext
{
    public TutorDbContext(DbContextOptions<TutorDbContext> options) : base(options) { }

    public DbSet<Tutor> Tutors { get; set; }
    public DbSet<TutorCourse> TutorCourses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assignmentDate = new DateTime(2025, 6, 4, 11, 58, 0, DateTimeKind.Utc);
        modelBuilder.Entity<TutorCourse>()
                .HasOne(tc => tc.Tutor)
                .WithMany(t => t.TutorCourses)
                .HasForeignKey(tc => tc.TutorId);

        // Seed initial data
        var tutor1Id = Guid.Parse("cdf8756b-1b69-49b3-b89a-a7ac0652b080");
        var course1Id = Guid.Parse("6daa15cc-f355-42e7-99aa-9a52086350a7");
        var course2Id = Guid.Parse("0365eed4-e67d-460a-abd7-6742b3698d86");

        // Seed Tutors
        modelBuilder.Entity<Tutor>().HasData(
            new Tutor { Id = tutor1Id, UserId = Guid.Parse("b2c3d4e5-f6a7-890b-cdef-1234567890ab") }
        );

        // Seed TutorCourses
        modelBuilder.Entity<TutorCourse>().HasData(
            new TutorCourse
            {
                Id = Guid.Parse("7c7753bb-90ad-48f6-8260-4fa8ef82322f"),
                TutorId = tutor1Id,
                CourseId = course1Id,
                AssignmentDate = assignmentDate //DateTime.UtcNow
            },
            new TutorCourse
            {
                Id = Guid.Parse("70466362-d316-4385-a0b5-b5df1eb8b779"),
                TutorId = tutor1Id,
                CourseId = course2Id,
                AssignmentDate = assignmentDate //DateTime.UtcNow
            }
        );
    }
}