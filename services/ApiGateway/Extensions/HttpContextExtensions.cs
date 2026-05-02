using Microsoft.AspNetCore.Http;

namespace ApiGateway.Extensions;

public static class HttpContextExtensions
{
    public static string? GetBearerToken(this HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            return null;

        return authHeader["Bearer ".Length..].Trim();
    }
}