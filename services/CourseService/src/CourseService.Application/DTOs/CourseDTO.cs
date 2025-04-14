namespace CourseService.Application.DTOs;

public record CourseDTO(Guid Id, string Name, string Code, int MaxStudents);
//public class CourseDTO
//{
//    public Guid Id { get; set; }
//    public required string Name { get; set; }
//    public required string Code { get; set; }
//    public required int MaxStudents { get; set; }
//}