using AdminService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminService.Application.Interfaces;

public interface IUserServiceClient
{
    Task<IEnumerable<UserDTO>> GetUsersByRoleAsync(string role);
    Task<Guid> CreateUserAsync(UserDTO user);
}