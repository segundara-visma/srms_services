using EnrollmentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnrollmentService.Application.Interfaces;

public interface IUserServiceClient
{
    Task<UserDTO> GetUserByIdAsync(Guid userId);
    Task<IEnumerable<UserDTO>> GetUsersByRoleAsync(string role);
}