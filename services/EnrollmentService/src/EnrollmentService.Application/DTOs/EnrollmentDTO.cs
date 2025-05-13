using System;

namespace EnrollmentService.Application.DTOs;

public class EnrollmentDTO
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string Status { get; set; } // Keep as string for API response, convert in mapping
    public decimal? PaymentAmount { get; set; }
}