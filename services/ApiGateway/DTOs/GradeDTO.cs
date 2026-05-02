namespace ApiGateway.DTOs;

public record GradeDTO(
    Guid Id,
    Guid StudentId,
    Guid CourseId,
    decimal GradeValue,
    DateTime GradedAt
);