using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GradeService.Domain.Entities;
using GradeService.Application.Interfaces;
using GradeService.Infrastructure.Persistence;
using GradeService.Infrastructure.Repositories;

namespace GradeService.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<GradeDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IGradeRepository, GradeRepository>();
    }
}
