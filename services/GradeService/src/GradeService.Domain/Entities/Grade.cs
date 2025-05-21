namespace GradeService.Domain.Entities;

public class Grade
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; } // Links to userId from UserService
    public Guid CourseId { get; set; } // Links to CourseService
    public decimal GradeValue { get; set; } // e.g., 85.5 (out of 100)
    public DateTime DateGraded { get; set; }
    public string? Comments { get; set; } // Optional comments on the grade
}