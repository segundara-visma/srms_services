using AuthService.Domain.Entities;
using System;
using System.Threading.Tasks;
using AuthService.Application.DTOs;

namespace AuthService.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginUser(string email, string password);  // This method handles the business logic for user login
    Task<LoginResponse> RefreshToken(string refreshToken);
    Task Logout(string refreshToken);
}
