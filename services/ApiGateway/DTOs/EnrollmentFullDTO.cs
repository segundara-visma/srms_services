namespace ApiGateway.DTOs;

public record EnrollmentFullDTO(
    Guid EnrollmentId,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    string Status
);