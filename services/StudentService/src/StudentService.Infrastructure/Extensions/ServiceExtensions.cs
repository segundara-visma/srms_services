using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentService.Infrastructure.Persistence;
using StudentService.Infrastructure.Clients;
using StudentService.Application.Interfaces;
using StudentService.Infrastructure.Repositories;
using StudentService.Application.Services;

namespace StudentService.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure DbContext
            services.AddDbContext<StudentDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories and services
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IStudentService, StudentServiceImpl>();

            // Remove redundant HttpClient configuration (handled in Program.cs)
            return services;
        }
    }
}