using System;
using System.Collections.Generic;

namespace ReportService.Domain.Entities;

public class Report
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public DateTime GeneratedAt { get; set; }
    public decimal? GPA { get; set; }
    public string Status { get; set; }
    public List<ReportDetail> ReportDetails { get; set; }
}