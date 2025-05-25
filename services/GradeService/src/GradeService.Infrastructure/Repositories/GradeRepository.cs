using GradeService.Application.Interfaces;
using GradeService.Infrastructure.Persistence;
using GradeService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GradeService.Infrastructure.Repositories;

public class GradeRepository : IGradeRepository
{
    private readonly GradeDbContext _context;

    public GradeRepository(GradeDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Grade?> GetByIdAsync(Guid id)
    {
        return await _context.Grades
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Grade>> GetAllAsync()
    {
        return await _context.Grades
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Grade grade)
    {
        await _context.Grades.AddAsync(grade);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Grade>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Grades
            .AsNoTracking()
            .Where(g => g.StudentId == studentId)
            .ToListAsync();
    }
}