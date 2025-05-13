using EnrollmentService.Application.Interfaces;
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

    public async Task<Enrollment> GetByIdAsync(Guid id)
    {
        return await _context.Enrollments.FindAsync(id);
    }

    public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(Guid userId)
    {
        return await _context.Enrollments.Where(e => e.StudentId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Enrollment>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Enrollments.Where(e => e.CourseId == courseId).ToListAsync();
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
}