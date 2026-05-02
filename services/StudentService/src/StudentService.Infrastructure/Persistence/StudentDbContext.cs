using Microsoft.EntityFrameworkCore;
using StudentService.Domain.Entities;
using System;

namespace StudentService.Infrastructure.Persistence;

public class StudentDbContext : DbContext
{
    public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().HasKey(s => s.Id);
        modelBuilder.Entity<Student>().Property(s => s.UserId).IsRequired();

        // Seed data (optional)
        modelBuilder.Entity<Student>().HasData(
            new Student(new Guid("b1c2d3e4-f5f6-7890-abcd-ef1234567890")) // This is userId(user.id) from user service
            {
                Id = new Guid("28c58647-ddc6-43b4-b694-4c2a42cef6c5") // This is studentId(student.id)
            },
            new Student(new Guid("c2d3e4f5-f678-90ab-cdef-1234567890ab")) // This is userId(user.id) from user service
            {
                Id = new Guid("79841481-3f79-47eb-ba04-e215e6432c49") // This is studentId(student.id)
            }
        );
    }
}