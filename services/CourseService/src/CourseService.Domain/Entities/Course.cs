namespace CourseService.Domain.Entities;
public class Course
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }
    public int MaxStudents { get; private set; }

    // Constructor without Id (used in normal operation, generates a new Guid)
    public Course(string name, string code, int maxStudents)
    {
        Id = Guid.NewGuid();
        Name = name;
        Code = code;
        MaxStudents = maxStudents;
    }

    // Constructor with Id (used for seeding or testing)
    public Course(Guid id, string name, string code, int maxStudents)
    {
        Id = id;
        Name = name;
        Code = code;
        MaxStudents = maxStudents;
    }

    public void Update(string name, string code, int maxStudents)
    {
        Name = name;
        Code = code;
        MaxStudents = maxStudents;
    }
}