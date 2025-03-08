using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;
using System.Threading.Tasks;
using UserService.Application.Interfaces;
using UserService.Application.DTOs;
using Microsoft.AspNetCore.Identity;

namespace UserService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context; // Injected context

    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    // Get user by ID
    public async Task<User?> GetByIdAsync(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Role)  // Include Role
            .Include(u => u.Profile)
            .AsNoTracking() // Ensures the entity is not tracked
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null;

        return new User
        {
            Id = user.Id,
            Email = user.Email,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            PasswordHash = user.PasswordHash,
            Role = user.Role, // Make sure Role is properly included and serialized
            Profile = user.Profile
        };
    }

    // Update existing user
    public async Task UpdateAsync(User user)
    {
        // Fetch the existing user from the database
        var existingUser = await _context.Users
            .Include(u => u.Role)  // Include Role if needed
            .Include(u => u.Profile) // Include Profile if needed
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        if (existingUser == null)
        {
            throw new Exception("User not found.");
        }

        // Update user properties
        existingUser.Email = user.Email ?? existingUser.Email;
        existingUser.Firstname = user.Firstname ?? existingUser.Firstname;
        existingUser.Lastname = user.Lastname ?? existingUser.Lastname;

        existingUser.Profile.Address = user.Profile?.Address ?? existingUser.Profile.Address;
        existingUser.Profile.Phone = user.Profile?.Phone ?? existingUser.Profile.Phone;
        existingUser.Profile.City = user.Profile?.City ?? existingUser.Profile.City;
        existingUser.Profile.State = user.Profile?.State ?? existingUser.Profile.State;
        existingUser.Profile.ZipCode = user.Profile?.ZipCode ?? existingUser.Profile.ZipCode;
        existingUser.Profile.Country = user.Profile?.Country ?? existingUser.Profile.Country;
        existingUser.Profile.Nationality = user.Profile?.Nationality ?? existingUser.Profile.Nationality;
        existingUser.Profile.Bio = user.Profile?.Bio ?? existingUser.Profile.Bio;
        existingUser.Profile.FacebookUrl = user.Profile?.FacebookUrl ?? existingUser.Profile.FacebookUrl;
        existingUser.Profile.TwitterUrl = user.Profile?.TwitterUrl ?? existingUser.Profile.TwitterUrl;
        existingUser.Profile.LinkedInUrl = user.Profile?.LinkedInUrl ?? existingUser.Profile.LinkedInUrl;
        existingUser.Profile.InstagramUrl = user.Profile?.InstagramUrl ?? existingUser.Profile.InstagramUrl;
        existingUser.Profile.WebsiteUrl = user.Profile?.WebsiteUrl ?? existingUser.Profile.WebsiteUrl;

        // Save the changes
        await _context.SaveChangesAsync();
    }

    // Get user by Email
    public async Task<User?> GetByEmailAsync(string email)
    {
        //return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);  // EF method to find by Email
        return await _context.Users
            .Include(u => u.Role)  // Include the Role for the user
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    // Add a new user
    public async Task AddAsync(User user)
    {
        // If Profile is null, create a new Profile and update its properties.
        user.Profile = new Profile
        {
            Id = Guid.Empty,  // You can use a default value, such as Guid.Empty or any other default value for the Profile
            ProfilePictureUrl = string.Empty,  // Default empty string for URLs
            Address = string.Empty,
            Phone = string.Empty,
            City = string.Empty,
            State = string.Empty,
            ZipCode = string.Empty,
            Country = string.Empty,
            Nationality = string.Empty,
            Bio = string.Empty,
            FacebookUrl = string.Empty,
            TwitterUrl = string.Empty,
            LinkedInUrl = string.Empty,
            InstagramUrl = string.Empty,
            WebsiteUrl = string.Empty,
            UserId = user.Id  // Assigning the UserId
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    // Get role by roleID
    public async Task<Role?> GetRoleByIdAsync(int roleId)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == roleId);
    }

    // Get role by by roleName
    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        //return await _context.Roles.FirstOrDefaultAsync(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower().Equals(roleName.ToLower()));
    }

    // Get users linked to a specific role
    public async Task<IList<User>> GetUsersByRoleIdAsync(int roleId)
    {
        return await _context.Users
            .Where(u => u.RoleId == roleId)  // Filter by RoleId
            .Include(u => u.Role)  // Include the Role data
            .ToListAsync();
    }
}
