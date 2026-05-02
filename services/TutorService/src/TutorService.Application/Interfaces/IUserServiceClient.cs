using TutorService.Application.Common;
using TutorService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TutorService.Application.Interfaces;

public interface IUserServiceClient
{
    Task<UserDTO> GetUserByIdAsync(Guid userId);
    //Task<IEnumerable<UserDTO>> GetUsersByRoleAsync(string role);
    Task<UserDTO> UpdateUserAsync(Guid userId, UpdateRequestDTO request);
    Task<PaginatedResponse<UserDTO>> GetUsersByRoleAsync(string role, int page = 1, int pageSize = 10);
}