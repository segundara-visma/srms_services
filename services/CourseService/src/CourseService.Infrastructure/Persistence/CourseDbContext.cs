using CourseService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Infrastructure.Persistence;

public class CourseDbContext : DbContext
{
    public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options) { }

    public DbSet<Course> Courses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the Course entity
        modelBuilder.Entity<Course>()
            .HasKey(c => c.Id); // Primary key

        modelBuilder.Entity<Course>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100); // Example constraint

        modelBuilder.Entity<Course>()
            .Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(10); // Example constraint

        modelBuilder.Entity<Course>()
            .Property(c => c.MaxStudents)
            .IsRequired();

        // Optional: Seed default data for courses
        modelBuilder.Entity<Course>().HasData(
            // Specified fixed Guid values for the Id fields to ensure consistency in the seeded data
            // (random Guid.NewGuid() values would change on each migration, which isn’t ideal for seeding)
            new Course(new Guid("6daa15cc-f355-42e7-99aa-9a52086350a7"), "Mathematics 101", "MATH101", 30),
            new Course(new Guid("0365eed4-e67d-460a-abd7-6742b3698d86"), "Physics 101", "PHYS101", 40)
        );
    }
}