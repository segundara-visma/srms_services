using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReportService.Application.DTOs;

namespace ReportService.Application.Interfaces;

public interface IGradeServiceClient
{
    Task<IEnumerable<GradeDTO>> GetGradesByStudentAsync(Guid studentId);
}

//public class GradeDTO
//{
//    public Guid StudentId { get; set; }
//    public Guid CourseId { get; set; }
//    public decimal GradeValue { get; set; }
//    public DateTime DateGraded { get; set; }
//    public string? Comments { get; set; }
//}