using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorService.Domain.Entities;

namespace TutorService.Application.Interfaces
{
    public interface ITutorRepository
    {
        Task<Tutor> GetByIdAsync(Guid id);
        Task<Tutor> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Tutor>> GetAllAsync();
        Task AddAsync(Tutor tutor);
        Task UpdateAsync(Tutor tutor);
        Task<IEnumerable<TutorCourse>> GetCoursesByTutorIdAsync(Guid tutorId);
        Task AddTutorCourseAsync(TutorCourse tutorCourse);
    }
}