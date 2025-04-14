using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CourseService.Domain.Entities;
using CourseService.Application.Interfaces;
using CourseService.Infrastructure.Persistence;
using CourseService.Infrastructure.Repositories;

namespace CourseService.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<CourseDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ICourseRepository, CourseRepository>();
    }
}
