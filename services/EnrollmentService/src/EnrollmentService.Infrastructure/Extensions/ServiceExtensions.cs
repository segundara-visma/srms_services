using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EnrollmentService.Infrastructure.Persistence;
using EnrollmentService.Infrastructure.Clients;
using EnrollmentService.Application.Interfaces;
using EnrollmentService.Infrastructure.Repositories;
using EnrollmentService.Application.Services;

namespace EnrollmentService.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure DbContext
            services.AddDbContext<EnrollmentDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories and services
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            services.AddScoped<IEnrollmentService, EnrollmentServiceImpl>();

            // Remove redundant HttpClient configuration (handled in Program.cs)
            return services;
        }
    }
}