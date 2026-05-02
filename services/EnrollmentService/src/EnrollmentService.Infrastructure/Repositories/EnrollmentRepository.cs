using EnrollmentService.Application.Interfaces;
using EnrollmentService.Application.Common;
using EnrollmentService.Domain.Entities;
using EnrollmentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnrollmentService.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly EnrollmentDbContext _context;

    public EnrollmentRepository(EnrollmentDbContext context)
    {
        _context = context;
    }

    public async Task<Enrollment?> GetByIdAsync(Guid id)
    {
        return await _context.Enrollments.FindAsync(id);
    }

    //public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(Guid userId)
    //{
    //    return await _context.Enrollments.Where(e => e.StudentId == userId).ToListAsync();
    //}

    public async Task<PaginatedResult<Enrollment>> GetByStudentIdAsync(Guid userId, int page = 1, int pageSize = 10)
    {
        var query = _context.Enrollments.Where(e => e.StudentId == userId);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Enrollment> { Items = items, TotalCount = totalCount };
    }

    //public async Task<IEnumerable<Enrollment>> GetByCourseIdAsync(Guid courseId)
    //{
    //    return await _context.Enrollments.Where(e => e.CourseId == courseId).ToListAsync();
    //}

    public async Task<PaginatedResult<Enrollment>> GetByCourseIdAsync(Guid courseId, int page = 1, int pageSize = 10)
    {
        var query = _context.Enrollments.Where(e => e.CourseId == courseId);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Enrollment> { Items = items, TotalCount = totalCount };
    }

    public async Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId)
    {
        return await _context.Enrollments
            .AsNoTracking() // For read-only queries to improve performance
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
    }

    public async Task AddAsync(Enrollment enrollment)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Enrollment enrollment)
    {
        _context.Enrollments.Update(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Enrollment>> GetAllAsync()
    {
        return await _context.Enrollments
            .AsNoTracking()
            .ToListAsync();
    }
}