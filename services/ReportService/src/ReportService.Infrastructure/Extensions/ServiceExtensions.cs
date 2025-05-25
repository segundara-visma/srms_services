using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportService.Domain.Entities;
using ReportService.Application.Interfaces;
using ReportService.Infrastructure.Persistence;
using ReportService.Infrastructure.Repositories;

namespace ReportService.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ReportDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IReportRepository, ReportRepository>();
    }
}
