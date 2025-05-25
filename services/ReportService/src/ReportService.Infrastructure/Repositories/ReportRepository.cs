using ReportService.Application.Interfaces;
using ReportService.Infrastructure.Persistence;
using ReportService.Application.DTOs;
using ReportService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ReportService.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ReportDbContext _context;

    public ReportRepository(ReportDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Report?> GetByIdAsync(Guid id)
    {
        return await _context.Reports
            .Include(r => r.ReportDetails)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Report?> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Reports
            .Include(r => r.ReportDetails)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.StudentId == studentId);
    }

    public async Task AddAsync(Report report)
    {
        await _context.Reports.AddAsync(report);
        await _context.SaveChangesAsync();
    }

    public async Task AddDetailAsync(ReportDetail detail)
    {
        await _context.ReportDetails.AddAsync(detail);
        await _context.SaveChangesAsync();
    }
}