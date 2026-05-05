using UserService.API.Middleware;

namespace UserService.API.Extensions;

/// <summary>
/// Extension methods for configuring application middleware.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Registers the global exception handling middleware.
    /// </summary>
    public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}