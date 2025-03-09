using UserService.Domain.Entities;
using System;
using System.Threading.Tasks;
using UserService.Application.DTOs;

namespace UserService.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task RegisterUser(User user, string password, string role);  // This method handles the business logic for user registration
    Task<bool> ValidatePasswordAsync(Guid userId, string password);
    Task<UserResponse?> UpdateAsync(Guid id, UpdateRequest user);  // Update existing user
}
