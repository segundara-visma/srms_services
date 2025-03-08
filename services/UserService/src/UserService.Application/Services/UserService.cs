using UserService.Application.Interfaces;  // To use IUserRepository
using UserService.Domain.Entities;
using System.Threading.Tasks;
using BCrypt.Net;
using UserService.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace UserService.Application.Services;

public class UserServices : IUserService
{
    private readonly IUserRepository _userRepository;

    // Constructor injection
    public UserServices(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task RegisterUser(User user, string plainPassword, string roleName)
    {
        // Business logic: Check if user exists
        var existingUser = await _userRepository.GetByEmailAsync(user.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        // Hash the password using BCrypt before saving it
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

        // Fetch the role by name from the repository
        var role = await _userRepository.GetRoleByNameAsync(roleName);
        if (role == null)
        {
            throw new InvalidOperationException("Invalid role.");
        }

        // Assign the role to the user
        user.RoleId = role.Id;  // Assuming user has a foreign key RoleId property

        // If user doesn't exist, register the user
        await _userRepository.AddAsync(user);
    }

}
