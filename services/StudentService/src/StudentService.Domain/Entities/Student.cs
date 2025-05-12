namespace StudentService.Domain.Entities;

public class Student
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; } // Links to UserService's User
    public List<Enrollment> Enrollments { get; private set; } = new();

    // Constructor for creating a new student record
    public Student(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
    }

    // Constructor for seeding/testing
    public Student(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }

    // Method to enroll in a course
    public void Enroll(Guid courseId)
    {
        if (Enrollments.Any(e => e.CourseId == courseId))
            throw new InvalidOperationException("Student is already enrolled in this course.");

        Enrollments.Add(new Enrollment(Id, courseId));
    }
}