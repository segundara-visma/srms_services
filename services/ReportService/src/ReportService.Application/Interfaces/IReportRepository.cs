using ReportService.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ReportService.Application.Interfaces;

public interface IReportRepository
{
    Task<Report?> GetByIdAsync(Guid id);
    Task<Report?> GetByStudentIdAsync(Guid studentId);
    Task AddAsync(Report report);
    Task AddDetailAsync(ReportDetail detail);
}