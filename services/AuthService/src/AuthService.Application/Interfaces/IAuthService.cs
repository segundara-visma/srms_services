using AuthService.Domain.Entities;
using System;
using System.Threading.Tasks;
using AuthService.Application.DTOs;

namespace AuthService.Application.Interfaces;

public interface IAuthService
{
    Task RevokeTokenAsync(string token);
    Task<bool> IsTokenRevokedAsync(string tokenId);
    Task<LoginResponseDTO> LoginUser(string email, string password);  // This method handles the business logic for user login
    Task<LoginResponseDTO> RefreshToken(string refreshToken);
    Task Logout(string refreshToken);
}
