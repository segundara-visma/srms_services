using StudentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentService.Application.Interfaces;

public interface IUserServiceClient
{
    Task<UserDTO> GetUserByIdAsync(Guid userId);
    Task<IEnumerable<UserDTO>> GetUsersByRoleAsync(string role);
    Task<UserDTO> UpdateUserAsync(Guid userId, UpdateRequest request);
}