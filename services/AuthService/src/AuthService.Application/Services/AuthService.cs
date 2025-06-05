using AuthService.Application.Interfaces;  // To use IUserRepository
using AuthService.Domain.Entities;
using System.Threading.Tasks;
using BCrypt.Net;
using AuthService.Application.Helpers;  // Include the Helpers namespace
using AuthService.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Application.Services;

public class AuthServices : IAuthService
{
    private readonly IUserService _userService;  // Injecting IUserService
    private readonly IAuthRepository _authRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;  // Add this to interact with the refresh token database
    private readonly JWTTokenHelper _jwtTokenHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDistributedCache _cache; // For Redis

    // Constructor injection
    public AuthServices(IUserService userService, IAuthRepository authRepository, IRefreshTokenRepository refreshTokenRepository, JWTTokenHelper jwtTokenHelper, IHttpContextAccessor httpContextAccessor, IDistributedCache cache)
    {
        _userService = userService;
        _authRepository = authRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenHelper = jwtTokenHelper;  // Inject JWTTokenHelper
        _httpContextAccessor = httpContextAccessor;  // Inject IHttpContextAccessor
        _cache = cache;
    }

    // Revoke a token by storing its tokenId (jti) in the cache with a short TTL (Time-To-Live)
    public async Task RevokeTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var tokenId = jwtToken?.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;

            if (tokenId == null)
            {
                throw new InvalidOperationException("Token ID (jti) missing from token.");
            }

            // Store the tokenId in the cache with a short TTL
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            await _cache.SetStringAsync(tokenId, "revoked", options); // Storing tokenId in the cache
        }
        catch (SecurityTokenMalformedException ex)
        {
            throw new InvalidOperationException("Malformed token", ex);
        }
    }

    // Check if the token is revoked by checking its tokenId (jti) in the cache
    public async Task<bool> IsTokenRevokedAsync(string token)
    {
        try
        {
            // Extract tokenId (jti) from the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var tokenId = jwtToken?.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;

            if (tokenId == null)
            {
                throw new InvalidOperationException("Token ID (jti) missing from token.");
            }

            // Check if the tokenId exists in the cache (i.e., if the token is revoked)
            var revokedToken = await _cache.GetStringAsync(tokenId);
            return revokedToken != null;
        }
        catch (SecurityTokenMalformedException)
        {
            throw new InvalidOperationException("The token is malformed and cannot be read.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while checking if the token is revoked.", ex);
        }
    }

    // Login logic
    public async Task<LoginResponse> LoginUser(string email, string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(plainPassword))
        {
            throw new ArgumentException("Email and password cannot be empty.");
        }

        // Send a request to the User Service to get the user by email
        var user = await _userService.GetUserByEmailAsync(email);

        if (user == null)
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        // Validate the password by calling UserService
        var isValidPassword = await _userService.ValidatePasswordAsync(user.Id, plainPassword);

        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // Generate both Access Token and Refresh Token
        var accessToken = _jwtTokenHelper.GenerateAccessToken(user.Id, email, user.Role);
        var refreshToken = _jwtTokenHelper.GenerateRefreshToken(user.Id, email);

        // Save the refresh token to the database or a secure store for later validation
        await _authRepository.SaveRefreshTokenAsync(user.Id, email, refreshToken);

        // Return access token in the response
        var response = new LoginResponse
        {
            Token = accessToken,
            Id = user.Id
        };

        // Send refresh token as HTTP-only cookie
        SetRefreshTokenCookie(refreshToken);

        return response;
    }


    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,  // Makes the cookie not accessible via JavaScript
            Secure = true,    // Ensures it is sent only over HTTPS
            SameSite = SameSiteMode.Strict,  // Prevents CSRF attacks
            Expires = DateTime.UtcNow.AddDays(7)  // Set expiration for 7 days
        };

        // Send the refresh token in a cookie
        // Access HttpContext through IHttpContextAccessor
        _httpContextAccessor.HttpContext.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
    }

    public async Task<LoginResponse> RefreshToken(string refreshToken)
    {
        // Validate the refresh token and extract the user information
        var principal = _jwtTokenHelper.ValidateRefreshToken(refreshToken);

        var email = principal.FindFirst("email")?.Value;

        // Send a request to the User Service to get the user by email
        var user = await _userService.GetUserByEmailAsync(email);

        if (user == null)
        {
            throw new InvalidOperationException("Invalid refresh token.");
        }

        // Generate a new access token and refresh token
        var newAccessToken = _jwtTokenHelper.GenerateAccessToken(user.Id, email, user.Role);
        var newRefreshToken = _jwtTokenHelper.GenerateRefreshToken(user.Id, email);

        // Save the new refresh token securely (optional step)
        await _authRepository.SaveRefreshTokenAsync(user.Id, email, newRefreshToken);

        // Send refresh token as HTTP-only cookie
        SetRefreshTokenCookie(newRefreshToken);

        // Return both new tokens in the response
        return new LoginResponse
        {
            Token = newAccessToken,
            Id = user.Id
        };
    }


    public async Task Logout(string refreshTokenFromCookie)
    {
        // Get the refresh token from the cookie or from the HttpContext
        var httpContext = _httpContextAccessor.HttpContext;

        if (refreshTokenFromCookie != null)
        {
            // Remove the refresh token from the database by setting its IsActive to false
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshTokenFromCookie);

            if (refreshToken != null && refreshToken.IsActive)
            {
                // Mark the refresh token as inactive in the database
                refreshToken.IsActive = false;
                await _refreshTokenRepository.UpdateAsync(refreshToken); // Save the change
            }
        }

        // Clear the refresh token from the cookie
        httpContext.Response.Cookies.Delete("RefreshToken");
    }

}
