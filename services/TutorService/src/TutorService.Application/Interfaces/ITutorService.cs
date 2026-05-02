using TutorService.Application.Common;
using TutorService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TutorService.Application.Interfaces
{
    public interface ITutorService
    {
        Task<TutorDTO> GetTutorByIdAsync(Guid tutorId);
        //Task<IEnumerable<TutorDTO>> GetAllTutorsAsync();
        Task<bool> AssignGradeAsync(Guid studentId, Guid courseId, decimal grade);
        //Task<IEnumerable<Guid>> GetAssignedCoursesAsync(Guid tutorId);
        Task<PaginatedResponse<TutorCoursesDTO>> GetAssignedCoursesAsync(Guid tutorId, int page = 1, int pageSize = 10);
        Task AssignCourseToTutorAsync(Guid tutorId, Guid courseId);
        Task CreateTutorAsync(Guid userId);
        Task<TutorDTO> UpdateTutorAsync(Guid userId, UpdateRequestDTO request);
        Task<PaginatedResponse<TutorDTO>> GetAllTutorsAsync(int page = 1, int pageSize = 10);
    }
}