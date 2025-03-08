using Microsoft.EntityFrameworkCore;  // For DbContextOptionsBuilder and EF Core extensions
using Microsoft.EntityFrameworkCore.Design;  // For IDesignTimeDbContextFactory<T> interface
using Microsoft.Extensions.Configuration;  // For reading appsettings.json configuration
using System.IO;  // For accessing the directory structure (SetBasePath)

namespace UserService.Infrastructure.Persistence;

public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();

        // Read the connection string from appsettings.json or from environment variables
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "UserService.Api"))
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);

        return new UserDbContext(optionsBuilder.Options);
    }
}
