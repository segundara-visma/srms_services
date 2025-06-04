using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TutorService.Domain.Entities;
using TutorService.Application.Interfaces;
using TutorService.Infrastructure.Persistence;
using TutorService.Infrastructure.Repositories;

namespace TutorService.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<TutorDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ITutorRepository, TutorRepository>();
    }
}
