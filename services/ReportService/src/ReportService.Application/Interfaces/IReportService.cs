using ReportService.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace ReportService.Application.Interfaces;

public interface IReportService
{
    Task<ReportDTO?> GetReportByIdAsync(Guid id);
    Task<ReportDTO?> GetReportByStudentIdAsync(Guid studentId);
    Task<ReportDTO> GenerateReportAsync(Guid studentId);
}