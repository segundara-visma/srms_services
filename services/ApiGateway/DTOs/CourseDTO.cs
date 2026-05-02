namespace ApiGateway.DTOs;

public record CourseDTO(
    Guid Id,
    string Name,
    string Code
);