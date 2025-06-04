using System;
using System.Collections.Generic;

namespace TutorService.Domain.Entities;

public class Tutor
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ICollection<TutorCourse> TutorCourses { get; set; } = new List<TutorCourse>();
}