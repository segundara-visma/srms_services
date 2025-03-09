using System.IdentityModel.Tokens.Jwt;
using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AuthService.Application.Middleware;

public class TokenRevocationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenRevocationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        // Skip token validation for login and logout endpoints
        if (context.Request.Path.StartsWithSegments("/api/Auth/login") ||
            context.Request.Path.StartsWithSegments("/api/Auth/logout"))
        {
            await _next(context);  // Skip token revocation check for login route
            return;
        }

        try
        {
            // Extract the token from the Authorization header
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Authorization header missing or invalid.");
                return;
            }

            // Extract the token
            var token = authorizationHeader.Replace("Bearer ", "");

            // Extract the token ID (jti) from the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var tokenId = jwtToken?.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;

            if (tokenId == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token ID (jti) missing.");
                return;
            }

            // Check if the token is revoked
            if (await authService.IsTokenRevokedAsync(tokenId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is revoked.");
                return;
            }
        }
        catch (Exception ex)
        {
            // Log exception for debugging purposes
            Console.Error.WriteLine($"Error in TokenRevocationMiddleware: {ex.Message}");
            throw;  // Rethrow to ensure proper handling of unhandled exceptions
        }

        await _next(context);  // Continue processing the request
    }
}