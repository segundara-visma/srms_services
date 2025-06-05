using AdminService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminService.Application.Interfaces;

public interface ITutorServiceClient
{
    Task<IEnumerable<TutorDTO>> GetAllTutorsAsync();
    Task CreateTutorAsync(Guid userId);
    Task AssignCourseToTutorAsync(Guid tutorId, Guid courseId);
}