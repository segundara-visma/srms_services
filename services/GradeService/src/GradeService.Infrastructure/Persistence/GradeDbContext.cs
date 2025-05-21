using Microsoft.EntityFrameworkCore;
using GradeService.Domain.Entities;

namespace GradeService.Infrastructure.Persistence;

public class GradeDbContext : DbContext
{
    public GradeDbContext(DbContextOptions<GradeDbContext> options) : base(options) { }

    public DbSet<Grade> Grades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Grade>().HasKey(g => g.Id);

        // Seed data with current date and time (08:59 PM EEST, May 20, 2025)
        var dateGraded = new DateTime(2025, 5, 20, 17, 59, 0, DateTimeKind.Utc); // 08:59 PM EEST (UTC+3)
        modelBuilder.Entity<Grade>().HasData(
            new Grade
            {
                Id = new Guid("c53ef888-3f6e-4f72-8cf4-da1e5f03ca16"),
                StudentId = new Guid("b1c2d3e4-f5f6-7890-abcd-ef1234567890"),
                CourseId = new Guid("0365eed4-e67d-460a-abd7-6742b3698d86"),
                GradeValue = 90.0m,
                DateGraded = dateGraded,
                Comments = "Excellent performance"
            }
        );
    }
}