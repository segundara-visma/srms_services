using UserService.Domain.Entities;
using System;
using System.Threading.Tasks;
using UserService.Application.Common;
using UserService.Application.DTOs;
using System.Collections.Generic;

namespace UserService.Application.Interfaces;

public interface IUserService
{
    Task<UserResponseDTO?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task RegisterUser(User user, string password, string role);  // This method handles the business logic for user registration
    Task<bool> ValidatePasswordAsync(Guid userId, string password);
    Task<UserResponseDTO?> UpdateAsync(Guid id, UpdateRequestDTO user);  // Update existing user
    Task<IEnumerable<UserResponseDTO>> GetUsersByRoleAsync(string role); // This is needed by the StudentService
    Task<PaginatedResponse<UserResponseDTO>> GetUsersByRoleAsync(string role, int page = 1, int pageSize = 10);
    Task<List<UsersInBatchDTO>> GetByIdsAsync(List<Guid> ids);
}
