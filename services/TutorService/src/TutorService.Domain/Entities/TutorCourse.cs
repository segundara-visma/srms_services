using System;

namespace TutorService.Domain.Entities;

public class TutorCourse
{
    public Guid Id { get; set; }
    public Guid TutorId { get; set; }
    public Guid CourseId { get; set; }
    public Tutor Tutor { get; set; }
    public DateTime AssignmentDate { get; set; }
}