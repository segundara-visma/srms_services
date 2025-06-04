using TutorService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TutorService.Application.Interfaces
{
    public interface ITutorService
    {
        Task<TutorDTO> GetTutorByIdAsync(Guid tutorId);
        Task<IEnumerable<TutorDTO>> GetAllTutorsAsync();
        Task<bool> AssignGradeAsync(Guid studentId, Guid courseId, decimal grade);
        Task<IEnumerable<Guid>> GetAssignedCoursesAsync(Guid tutorId);
        Task AssignCourseToTutorAsync(Guid tutorId, Guid courseId);
    }
}