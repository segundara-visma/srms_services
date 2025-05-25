using System;
using System.Collections.Generic;

namespace ReportService.Application.DTOs;

public class ReportDTO
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public DateTime GeneratedAt { get; set; }
    public decimal? GPA { get; set; }
    public string Status { get; set; }
    public IEnumerable<ReportDetailDTO> Details { get; set; }
}