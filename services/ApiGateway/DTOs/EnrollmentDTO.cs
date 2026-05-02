namespace ApiGateway.DTOs;

public record EnrollmentDTO(
    Guid Id,
    Guid StudentId,
    Guid CourseId,
    DateTime EnrollmentDate,
    string Status,
    decimal? PaymentAmount
);