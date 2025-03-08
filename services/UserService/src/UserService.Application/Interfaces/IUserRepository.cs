using UserService.Domain.Entities;
using UserService.Application.DTOs;

namespace UserService.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);  // Retrieve by GUID (unique identifier)
    Task<User?> GetByEmailAsync(string email);  // Retrieve by Email
    Task AddAsync(User user);  // Add a new user
    Task UpdateAsync(User user);  // Update existing user

    // New methods for working with roles and users
    Task<Role?> GetRoleByIdAsync(int roleId);  // Get a role by its ID
    Task<Role?> GetRoleByNameAsync(string roleName);  // Get a role by its name
    Task<IList<User>> GetUsersByRoleIdAsync(int roleId);  // Get all users assigned to a specific role
}
