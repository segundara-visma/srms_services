using StudentService.Application.Interfaces;
using StudentService.Domain.Entities;
using StudentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentService.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly StudentDbContext _context;

    public StudentRepository(StudentDbContext context)
    {
        _context = context;
    }

    public async Task<Student> GetByIdAsync(Guid id)
    {
        return await _context.Students
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student> GetByUserIdAsync(Guid userId)
    {
        return await _context.Students
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students
            .Include(s => s.Enrollments)
            .ToListAsync();
    }
}