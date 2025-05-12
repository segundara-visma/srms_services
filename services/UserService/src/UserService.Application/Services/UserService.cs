using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using System.Threading.Tasks;
using BCrypt.Net;
using UserService.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace UserService.Application.Services;

public class UserServices : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserServices(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
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
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        user.Firstname = request.Firstname ?? user.Firstname;
        user.Lastname = request.Lastname ?? user.Lastname;
        user.Email = request.Email ?? user.Email;

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
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<bool> ValidatePasswordAsync(Guid id, string password)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    public async Task RegisterUser(User user, string plainPassword, string roleName)
    {
        var existingUser = await _userRepository.GetByEmailAsync(user.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

        var role = await _userRepository.GetRoleByNameAsync(roleName);
        if (role == null)
        {
            throw new InvalidOperationException("Invalid role.");
        }

        user.RoleId = role.Id;

        await _userRepository.AddAsync(user);
    }

    public async Task<IEnumerable<UserResponse>> GetUsersByRoleAsync(string role)
    {
        var roleEntity = await _userRepository.GetRoleByNameAsync(role);
        if (roleEntity == null)
        {
            return Enumerable.Empty<UserResponse>(); // Return empty list if role not found
        }

        var users = await _userRepository.GetUsersByRoleIdAsync(roleEntity.Id);
        return users.Select(user => new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Role = user.Role.Name,
            Profile = user.Profile
        });
    }
}