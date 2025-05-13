using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EnrollmentService.Infrastructure.Persistence;

public class EnrollmentDbContextFactory : IDesignTimeDbContextFactory<EnrollmentDbContext>
{
    public EnrollmentDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EnrollmentDbContext>();

        // Read the connection string from appsettings.json or environment variables
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "EnrollmentService.API"))
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);

        return new EnrollmentDbContext(optionsBuilder.Options);
    }
}