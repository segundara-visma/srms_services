using CourseService.Application.Interfaces;
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

    public async Task<Course> GetByIdAsync(Guid id)
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
}