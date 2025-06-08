using AdminService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminService.Application.Interfaces;

public interface IUserServiceClient
{
    Task<IEnumerable<AdminDTO>> GetUsersByRoleAsync(string role);
    Task<Guid> CreateUserAsync(UserDTO user);
    Task<AdminDTO> UpdateUserAsync(Guid userId, UpdateRequest request);
    Task<AdminDTO> GetUserByIdAsync(Guid userId);
}