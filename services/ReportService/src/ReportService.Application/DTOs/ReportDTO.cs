using System;
using System.Collections.Generic;

namespace ReportService.Application.DTOs;

public record ReportDTO(Guid Id, Guid StudentId, DateTime GeneratedAt, decimal? GPA, string Status, IEnumerable<ReportDetailDTO> Details);


//public class ReportDTO
//{
//    public Guid Id { get; set; }
//    public Guid StudentId { get; set; }
//    public DateTime GeneratedAt { get; set; }
//    public decimal? GPA { get; set; }
//    public string Status { get; set; }
//    public IEnumerable<ReportDetailDTO> Details { get; set; }
//}