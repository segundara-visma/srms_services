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

    public async Task<UserResponse?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);  // Repository method

        if (user == null) return null;

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Role = user.Role.Name,
            Profile = user.Profile
        };
    }

    public async Task<UserResponse?> UpdateAsync(Guid id, UpdateRequest request)
    {
        // Repository method
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        // Map the properties from the UpdateRequest to the User entity
        user.Firstname = request.Firstname ?? user.Firstname;
        user.Lastname = request.Lastname ?? user.Lastname;
        user.Email = request.Email ?? user.Email;

        // Update the existing Profile
        user.Profile.Address = request.Address ?? user.Profile.Address;
        user.Profile.Phone = request.Phone ?? user.Profile.Phone;
        user.Profile.City = request.City ?? user.Profile.City;
        user.Profile.State = request.State ?? user.Profile.State;
        user.Profile.ZipCode = request.ZipCode ?? user.Profile.ZipCode;
        user.Profile.Country = request.Country ?? user.Profile.Country;
        user.Profile.Nationality = request.Nationality ?? user.Profile.Nationality;
        user.Profile.Bio = request.Bio ?? user.Profile.Bio;
        user.Profile.FacebookUrl = request.FacebookUrl ?? user.Profile.FacebookUrl;
        user.Profile.TwitterUrl = request.TwitterUrl ?? user.Profile.TwitterUrl;
        user.Profile.LinkedInUrl = request.LinkedInUrl ?? user.Profile.LinkedInUrl;
        user.Profile.InstagramUrl = request.InstagramUrl ?? user.Profile.InstagramUrl;
        user.Profile.WebsiteUrl = request.WebsiteUrl ?? user.Profile.WebsiteUrl;

        // Submit the UpdateRequest to the repository
        await _userRepository.UpdateAsync(user);

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Role = user.Role.Name,
            Profile = user.Profile
        };
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);  // Repository method
    }

    public async Task<bool> ValidatePasswordAsync(Guid id, string password)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null) return false;

        // Verify the password
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            //throw new InvalidOperationException("Invalid email or password.");
            return false;
        }

        return true;
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
