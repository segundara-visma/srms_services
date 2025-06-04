using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Infrastructure.Persistence;

namespace TutorService.Infrastructure.Repositories
{
    public class TutorRepository : ITutorRepository
    {
        private readonly TutorDbContext _context;

        public TutorRepository(TutorDbContext context)
        {
            _context = context;
        }

        public async Task<Tutor> GetByIdAsync(Guid id)
        {
            return await _context.Tutors
                .Include(t => t.TutorCourses)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tutor> GetByUserIdAsync(Guid userId)
        {
            return await _context.Tutors
                .Include(t => t.TutorCourses)
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<IEnumerable<Tutor>> GetAllAsync()
        {
            return await _context.Tutors
                .Include(t => t.TutorCourses)
                .ToListAsync();
        }

        public async Task AddAsync(Tutor tutor)
        {
            await _context.Tutors.AddAsync(tutor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tutor tutor)
        {
            _context.Tutors.Update(tutor);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TutorCourse>> GetCoursesByTutorIdAsync(Guid tutorId)
        {
            return await _context.TutorCourses
                .Where(tc => tc.TutorId == tutorId)
                .ToListAsync();
        }

        public async Task AddTutorCourseAsync(TutorCourse tutorCourse)
        {
            await _context.TutorCourses.AddAsync(tutorCourse);
            await _context.SaveChangesAsync();
        }
    }
}