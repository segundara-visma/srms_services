using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces;

public interface IAuthRepository
{
    Task SaveRefreshTokenAsync(Guid userId, string email, string refreshToken);  // New method to store the refresh token
}
