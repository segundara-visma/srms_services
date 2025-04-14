namespace CourseService.Application.DTOs;

public record UpdateCourseDTO(string Name, string Code, int MaxStudents);
//public class UpdateCourseDTO
//{
//    public required string Name { get; set; }
//    public required string Code { get; set; }
//    public required int MaxStudents { get; set; }
//}