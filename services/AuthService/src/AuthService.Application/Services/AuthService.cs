using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Application.Helpers;
using AuthService.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Application.Services;

public class AuthServices : IAuthService
{
    private readonly IUserService _userService;
    private readonly IAuthRepository _authRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JWTTokenHelper _jwtTokenHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDistributedCache _cache;

    public AuthServices(
        IUserService userService,
        IAuthRepository authRepository,
        IRefreshTokenRepository refreshTokenRepository,
        JWTTokenHelper jwtTokenHelper,
        IHttpContextAccessor httpContextAccessor,
        IDistributedCache cache)
    {
        _userService = userService;
        _authRepository = authRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenHelper = jwtTokenHelper;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
    }

    // -----------------------------
    // LOGIN
    // -----------------------------
    public async Task<LoginResponseDTO> LoginUser(string email, string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Email and password cannot be empty.");

        var user = await _userService.GetUserByEmailAsync(email);

        if (user == null)
            throw new InvalidOperationException("Invalid email or password.");

        var isValidPassword = await _userService.ValidatePasswordAsync(user.Id, plainPassword);

        if (!isValidPassword)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var accessToken = _jwtTokenHelper.GenerateAccessToken(user.Id, email, user.Role);
        var refreshToken = _jwtTokenHelper.GenerateRefreshToken(user.Id, email);

        await _authRepository.SaveRefreshTokenAsync(user.Id, email, refreshToken);

        SetRefreshTokenCookie(refreshToken);

        // RECORD USAGE (positional constructor)
        return new LoginResponseDTO(accessToken, user.Id);
    }

    // -----------------------------
    // REFRESH TOKEN
    // -----------------------------
    public async Task<LoginResponseDTO> RefreshToken(string refreshToken)
    {
        var principal = _jwtTokenHelper.ValidateRefreshToken(refreshToken);

        var email = principal.FindFirst("email")?.Value;

        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidOperationException("Invalid refresh token.");

        var user = await _userService.GetUserByEmailAsync(email);

        if (user == null)
            throw new InvalidOperationException("Invalid refresh token.");

        var newAccessToken = _jwtTokenHelper.GenerateAccessToken(user.Id, email, user.Role);
        var newRefreshToken = _jwtTokenHelper.GenerateRefreshToken(user.Id, email);

        await _authRepository.SaveRefreshTokenAsync(user.Id, email, newRefreshToken);

        SetRefreshTokenCookie(newRefreshToken);

        // RECORD USAGE
        return new LoginResponseDTO(newAccessToken, user.Id);
    }

    // -----------------------------
    // LOGOUT
    // -----------------------------
    public async Task Logout(string refreshTokenFromCookie)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (!string.IsNullOrEmpty(refreshTokenFromCookie))
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshTokenFromCookie);

            if (refreshToken != null && refreshToken.IsActive)
            {
                refreshToken.IsActive = false;
                await _refreshTokenRepository.UpdateAsync(refreshToken);
            }
        }

        httpContext?.Response.Cookies.Delete("RefreshToken");
    }

    // -----------------------------
    // TOKEN REVOCATION
    // -----------------------------
    public async Task RevokeTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var tokenId = jwtToken?.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;

            if (tokenId == null)
                throw new InvalidOperationException("Token ID (jti) missing from token.");

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            await _cache.SetStringAsync(tokenId, "revoked", options);
        }
        catch (SecurityTokenMalformedException ex)
        {
            throw new InvalidOperationException("Malformed token", ex);
        }
    }

    public async Task<bool> IsTokenRevokedAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var tokenId = jwtToken?.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;

            if (tokenId == null)
                throw new InvalidOperationException("Token ID (jti) missing from token.");

            var revokedToken = await _cache.GetStringAsync(tokenId);
            return revokedToken != null;
        }
        catch (SecurityTokenMalformedException)
        {
            throw new InvalidOperationException("The token is malformed and cannot be read.");
        }
    }

    // -----------------------------
    // COOKIE HELPERS
    // -----------------------------
    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        _httpContextAccessor.HttpContext?.Response
            .Cookies.Append("RefreshToken", refreshToken, cookieOptions);
    }
}