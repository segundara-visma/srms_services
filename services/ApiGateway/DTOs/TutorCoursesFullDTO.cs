namespace ApiGateway.DTOs;

public record TutorCoursesFullDTO(
    Guid TutorId,
    Guid CourseId,
    string CourseTitle,
    string CourseCode
);