using System;
using System.Collections.Generic;

namespace ReportService.Application.DTOs;

public class ReportDetailDTO
{
    public Guid CourseId { get; set; }
    public decimal? Grade { get; set; }
    public string CourseTitle { get; set; }
    public int Credits { get; set; }
    public string Status { get; set; }
}