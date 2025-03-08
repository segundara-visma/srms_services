using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence;
using System.Threading.Tasks;
using AuthService.Application.Interfaces;

namespace AuthService.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AuthDbContext _context; // Injected context
    private readonly IUserService _userService; // Injecting IUserService

    public AuthRepository(AuthDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    // Save the refresh token for a user
    public async Task SaveRefreshTokenAsync(Guid userId, string email, string refreshToken)
    {
        // Send a request to the User Service to get the user by email
        var user = await _userService.GetUserByEmailAsync(email);

        //var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            var refreshTokenEntity = new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                ExpiresAt = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(7), DateTimeKind.Utc),  // Ensure UTC
                IsActive = true
            };

            _context.RefreshTokens.Add(refreshTokenEntity);  // Assuming a DbSet<RefreshToken> exists
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException("User not found.");
        }
    }
}
