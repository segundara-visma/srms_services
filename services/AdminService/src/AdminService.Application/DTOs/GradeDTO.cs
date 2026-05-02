namespace AdminService.Application.DTOs;

public record GradeDTO(Guid Id, Guid StudentId, Guid CourseId, decimal GradeValue, DateTime GradedAt, string? Comments);

//public class GradeDTO
//{
//    public Guid Id { get; set; }
//    public Guid StudentId { get; set; }
//    public Guid CourseId { get; set; }
//    public decimal GradeValue { get; set; }
//    public DateTime DateGraded { get; set; }
//    public string? Comments { get; set; }
//}