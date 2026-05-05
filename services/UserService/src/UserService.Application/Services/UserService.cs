using UserService.Application.Interfaces;
using UserService.Application.Exceptions;
using UserService.Domain.Entities;
using System.Threading.Tasks;
using BCrypt.Net;
using UserService.Application.Common;
using UserService.Application.DTOs;
using UserService.Application.Mappers;
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

    public async Task<UserResponseDTO?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new ApiException("User not found", 404);

        return UserMapper.ToDto(user);
    }

    public async Task<UserResponseDTO?> UpdateAsync(Guid id, UpdateRequestDTO request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new ApiException("User not found", 404);

        user.Firstname = request.Firstname ?? user.Firstname;
        user.Lastname = request.Lastname ?? user.Lastname;
        user.Email = request.Email ?? user.Email;

        // Ensure Profile exists
        user.Profile ??= new Profile();

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

        return UserMapper.ToDto(user);
    }

    public async Task<UserResponseDTO?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            throw new ApiException("User not found", 404);

        return UserMapper.ToDto(user);
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
            throw new ApiException("Email already exists.", 400);
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

        var role = await _userRepository.GetRoleByNameAsync(roleName);
        if (role == null)
        {
            throw new ApiException("Invalid role.", 400);
        }

        user.RoleId = role.Id;

        await _userRepository.AddAsync(user);
    }

    public async Task<IEnumerable<UserResponseDTO>> GetUsersByRoleAsync(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ApiException("Role cannot be empty.", 400);

        var roleEntity = await _userRepository.GetRoleByNameAsync(role);

        if (roleEntity == null)
            throw new ApiException("Role not found.", 404);

        var users = await _userRepository.GetUsersByRoleIdAsync(roleEntity.Id);

        return users.Select(UserMapper.ToDto);
    }

    public async Task<PaginatedResponse<UserResponseDTO>> GetUsersByRoleAsync(string role, int page = 1, int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ApiException("Role cannot be empty.", 400);

        var roleEntity = await _userRepository.GetRoleByNameAsync(role);
        if (roleEntity == null)
            throw new ApiException("Role not found.", 404);

        var result = await _userRepository.GetUsersByRoleIdAsync(roleEntity.Id, page, pageSize);

        return new PaginatedResponse<UserResponseDTO>
        {
            Items = result.Items.Select(UserMapper.ToDto),
            TotalCount = result.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<List<UsersInBatchDTO>> GetByIdsAsync(List<Guid> ids)
    {
        var users = await _userRepository.GetByUserIdsAsync(ids);

        return users.Select(user => new UsersInBatchDTO(
            user.Firstname,
            user.Lastname,
            user.Email
        )).ToList();
    }
}