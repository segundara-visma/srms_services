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
            new Course(new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), "Mathematics 101", "MATH101", 30),
            new Course(new Guid("b2c3d4e5-f678-90ab-cdef-1234567890ab"), "Physics 101", "PHYS101", 40)
        );
    }
}