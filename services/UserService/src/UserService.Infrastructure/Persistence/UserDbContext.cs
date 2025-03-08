using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Profile> Profiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the User entity
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();

        // Configure the one-to-many relationship between roles and users
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .IsRequired();

        // Configure the Role entity
        modelBuilder.Entity<Role>()
            .HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId);

        // One-to-one relationship between User and Profile
        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne()
            .HasForeignKey<Profile>(p => p.UserId)  // Define the foreign key
            .IsRequired(false);  // Optional profile, user can exist without a profile

        // Optional: Seeding default data for roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Tutor" },
            new Role { Id = 3, Name = "Student" }
        );
    }
}
