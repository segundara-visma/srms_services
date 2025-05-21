namespace GradeService.Application.DTOs;

public class GradeDTO
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public decimal GradeValue { get; set; }
    public DateTime DateGraded { get; set; }
    public string? Comments { get; set; }
}