using GradeService.Application.Interfaces;
using GradeService.Application.Common;
using GradeService.Infrastructure.Persistence;
using GradeService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GradeService.Infrastructure.Repositories;

public class GradeRepository : IGradeRepository
{
    private readonly GradeDbContext _context;

    public GradeRepository(GradeDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Grade?> GetByIdAsync(Guid id) =>
        await _context.Grades
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);

    public async Task<IEnumerable<Grade>> GetAllAsync() =>
        await _context.Grades
            .AsNoTracking()
            .ToListAsync();

    public async Task AddAsync(Grade grade)
    {
        await _context.Grades.AddAsync(grade);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Grade>> GetByStudentIdAsync(Guid studentId) =>
        await _context.Grades
            .AsNoTracking()
            .Where(g => g.StudentId == studentId)
            .ToListAsync();

    public async Task<PaginatedResult<Grade>> GetWithPaginationByStudentIdAsync(
        Guid userId,
        int page = 1,
        int pageSize = 10)
    {
        var query = _context.Grades.Where(g => g.StudentId == userId);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Grade>
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    public async Task<PaginatedResult<Grade>> GetWithPaginationByCourseIdAsync(
        Guid courseId,
        int page = 1,
        int pageSize = 10)
    {
        var query = _context.Grades.Where(g => g.CourseId == courseId);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Grade>
        {
            Items = items,
            TotalCount = totalCount
        };
    }
}