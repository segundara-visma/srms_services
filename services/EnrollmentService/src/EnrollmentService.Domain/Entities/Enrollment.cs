using System;

namespace EnrollmentService.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; } // Reference to User with Role "Student"
    public Guid CourseId { get; set; }  // Reference to Course
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; }  // Changed to EnrollmentStatus enum
    public decimal? PaymentAmount { get; set; } // Optional payment details
}