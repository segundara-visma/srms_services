using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdminService.Application.DTOs;

namespace AdminService.Application.Interfaces;

public interface IAdminService
{
    Task<IEnumerable<AdminDTO>> GetAllUsersByRoleAsync(string role);
    Task<Guid> CreateUserAsync(string firstName, string lastName, string email, string password, string role);
    Task<SystemOverviewDTO> GetSystemOverviewAsync();
    Task AssignCourseToTutorAsync(Guid tutorId, Guid courseId);
    Task<AdminDTO> UpdateAdminAsync(Guid userId, UpdateRequest request);
    Task<AdminDTO> GetAdminByIdAsync(Guid userId);
}