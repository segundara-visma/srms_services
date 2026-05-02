using CourseService.Application.Interfaces;
using CourseService.Application.Common;
using CourseService.Application.DTOs;
using CourseService.Domain.Entities;
using CourseService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseService.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly CourseDbContext _context;

    public CourseRepository(CourseDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _context.Courses.FindAsync(id);
    }

    public async Task UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _context.Courses.ToListAsync();
    }

    public async Task<PaginatedResult<Course>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var query = _context.Courses;

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Course> { Items = items, TotalCount = totalCount };
    }

    public async Task<List<Course>> GetByCourseIdsAsync(List<Guid> ids)
    {
        return await _context.Courses
            .AsNoTracking()
            .Where(c => ids.Contains(c.Id))
            .ToListAsync();
    }
}