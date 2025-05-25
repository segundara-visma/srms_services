using System;
using System.Collections.Generic;

namespace ReportService.Domain.Entities;

public class ReportDetail
{
    public Guid Id { get; set; }
    public Guid ReportId { get; set; }
    public Guid CourseId { get; set; }
    public decimal? Grade { get; set; }
    public string CourseTitle { get; set; }
    public int Credits { get; set; }
    public string Status { get; set; }
}