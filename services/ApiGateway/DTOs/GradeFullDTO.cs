namespace ApiGateway.DTOs;

public record GradeFullDTO(
    Guid GradeId,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
	decimal GradeValue
);