namespace GradeService.Application.DTOs;

public record CreateGradeDTO(
    Guid StudentId,
    Guid CourseId,
    decimal GradeValue,
    DateTime GradedAt,
    string? Comments
);