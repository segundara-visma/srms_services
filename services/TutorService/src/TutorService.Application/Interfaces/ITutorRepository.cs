using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorService.Domain.Entities;
using TutorService.Application.Common;
using TutorService.Application.DTOs;

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
        Task<PaginatedResult<TutorCourse>> GetPaginatedCoursesByTutorIdAsync(Guid tutorId, int page, int pageSize);
        Task AddTutorCourseAsync(TutorCourse tutorCourse);
        Task<List<Tutor>> GetByUserIdsAsync(List<Guid> userIds);
    }
}