namespace TutorService.Application.DTOs;

public record TutorCoursesDTO(
    Guid Id,
    Guid TutorId,
    Guid CourseId
);