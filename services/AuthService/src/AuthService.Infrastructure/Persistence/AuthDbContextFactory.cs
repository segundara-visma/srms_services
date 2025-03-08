using Microsoft.EntityFrameworkCore;  // For DbContextOptionsBuilder and EF Core extensions
using Microsoft.EntityFrameworkCore.Design;  // For IDesignTimeDbContextFactory<T> interface
using Microsoft.Extensions.Configuration;  // For reading appsettings.json configuration
using System.IO;  // For accessing the directory structure (SetBasePath)

namespace AuthService.Infrastructure.Persistence;

public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();

        // Read the connection string from appsettings.json or from environment variables
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "AuthService.API"))
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);

        return new AuthDbContext(optionsBuilder.Options);
    }
}
