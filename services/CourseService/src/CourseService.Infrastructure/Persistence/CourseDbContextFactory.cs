using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CourseService.Infrastructure.Persistence;

public class CourseDbContextFactory : IDesignTimeDbContextFactory<CourseDbContext>
{
    public CourseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CourseDbContext>();

        // Read the connection string from appsettings.json or environment variables
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "CourseService.API"))
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);

        return new CourseDbContext(optionsBuilder.Options);
    }
}