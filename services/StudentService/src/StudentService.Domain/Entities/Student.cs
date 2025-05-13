using System;

namespace StudentService.Domain.Entities;

public class Student
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public Student(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
    }

    // Parameterless constructor for EF Core
    private Student() { }
}