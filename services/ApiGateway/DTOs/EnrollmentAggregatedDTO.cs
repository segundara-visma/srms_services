namespace ApiGateway.DTOs;

public record EnrollmentAggregatedDTO(
    Guid Id,
    StudentDTO Student,
    CourseDTO Course,
    DateTime EnrollmentDate,
    string Status,
    decimal? PaymentAmount
);