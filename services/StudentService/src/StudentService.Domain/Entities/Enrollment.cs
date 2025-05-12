namespace StudentService.Domain.Entities;

public class Enrollment
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }

    public Enrollment(Guid studentId, Guid courseId)
    {
        StudentId = studentId;
        CourseId = courseId;
    }
}