namespace CourseService.Application.DTOs;

public record CreateCourseDTO(string Name, string Code, int MaxStudents);
//public class CreateCourseDTO
//{
//    public required string Name { get; set; }
//    public required string Code { get; set; }
//    public required int MaxStudents { get; set; }
//}