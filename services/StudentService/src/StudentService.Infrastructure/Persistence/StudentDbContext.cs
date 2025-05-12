using Microsoft.EntityFrameworkCore;
using StudentService.Domain.Entities;

namespace StudentService.Infrastructure.Persistence
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.UserId)
                .IsUnique();

            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.StudentId, e.CourseId });

            modelBuilder.Entity<Enrollment>()
                .HasOne<Student>()
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId);

            // Seed some data
            var studentId1 = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
            var userId1 = new Guid("b1c2d3e4-f5f6-7890-abcd-ef1234567890");
            var studentId2 = new Guid("b2c3d4e5-f678-90ab-cdef-1234567890ab");
            var userId2 = new Guid("c2d3e4f5-f678-90ab-cdef-1234567890ab");

            modelBuilder.Entity<Student>().HasData(
                new Student(studentId1, userId1),
                new Student(studentId2, userId2)
            );

            modelBuilder.Entity<Enrollment>().HasData(
                new { StudentId = studentId1, CourseId = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890") }
            );
        }
    }
}