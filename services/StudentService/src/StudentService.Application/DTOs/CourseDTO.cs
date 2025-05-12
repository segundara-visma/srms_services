namespace StudentService.Application.DTOs;

public record CourseDTO(Guid Id, string Name, string Code, int MaxStudents);