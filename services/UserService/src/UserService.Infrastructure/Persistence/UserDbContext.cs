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
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.PasswordHash)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Firstname)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Lastname)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.RoleId)
            .IsRequired();

        // Configure the one-to-many relationship between roles and users
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .IsRequired();

        // Configure the Role entity
        modelBuilder.Entity<Role>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<Role>()
            .HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId);

        modelBuilder.Entity<Role>()
            .Property(r => r.Name)
            .IsRequired();

        // One-to-one relationship between User and Profile
        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne()
            .HasForeignKey<Profile>(p => p.UserId)
            .IsRequired(false); // Optional profile, user can exist without a profile

        modelBuilder.Entity<Profile>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Profile>()
            .Property(p => p.UserId)
            .IsRequired();

        // Optional fields in Profile (no IsRequired() as they are nullable)
        modelBuilder.Entity<Profile>()
            .Property(p => p.Address);

        modelBuilder.Entity<Profile>()
            .Property(p => p.Phone);

        modelBuilder.Entity<Profile>()
            .Property(p => p.ProfilePictureUrl);

        modelBuilder.Entity<Profile>()
            .Property(p => p.City);

        modelBuilder.Entity<Profile>()
            .Property(p => p.State);

        modelBuilder.Entity<Profile>()
            .Property(p => p.ZipCode);

        modelBuilder.Entity<Profile>()
            .Property(p => p.Country);

        modelBuilder.Entity<Profile>()
            .Property(p => p.Nationality);

        modelBuilder.Entity<Profile>()
            .Property(p => p.Bio);

        modelBuilder.Entity<Profile>()
            .Property(p => p.FacebookUrl);

        modelBuilder.Entity<Profile>()
            .Property(p => p.TwitterUrl);

        modelBuilder.Entity<Profile>()
            .Property(p => p.LinkedInUrl);

        modelBuilder.Entity<Profile>()
            .Property(p => p.InstagramUrl);

        modelBuilder.Entity<Profile>()
            .Property(p => p.WebsiteUrl);

        // Seed data for roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Tutor" },
            new Role { Id = 3, Name = "Student" }
        );

        // Seed data for users (matching StudentService UserIds)
        var userId1 = new Guid("b1c2d3e4-f5f6-7890-abcd-ef1234567890"); // Student
        var userId2 = new Guid("c2d3e4f5-f678-90ab-cdef-1234567890ab"); // Student
        var userId3 = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"); // Admin
        var userId4 = new Guid("b2c3d4e5-f6a7-890b-cdef-1234567890ab"); // Tutor

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = userId1,
                Email = "john.doe@example.com",
                PasswordHash = "$2a$12$FtTE2pZ1h7nRML.MFelVL.d6eyre70x295KX/T6kiaqW55v0Fdo2a", // Hashed password ("123")
                Firstname = "John",
                Lastname = "Doe",
                RoleId = 3 // Student role
            },
            new User
            {
                Id = userId2,
                Email = "jane.smith@example.com",
                PasswordHash = "$2a$12$FtTE2pZ1h7nRML.MFelVL.d6eyre70x295KX/T6kiaqW55v0Fdo2a", // Hashed password ("123")
                Firstname = "Jane",
                Lastname = "Smith",
                RoleId = 3 // Student role
            },
            new User
            {
                Id = userId3,
                Email = "admin.user@example.com",
                PasswordHash = "$2a$12$FtTE2pZ1h7nRML.MFelVL.d6eyre70x295KX/T6kiaqW55v0Fdo2a", // Hashed password ("123")
                Firstname = "Admin",
                Lastname = "User",
                RoleId = 1 // Admin role
            },
            new User
            {
                Id = userId4,
                Email = "tutor.teacher@example.com",
                PasswordHash = "$2a$12$FtTE2pZ1h7nRML.MFelVL.d6eyre70x295KX/T6kiaqW55v0Fdo2a", // Hashed password ("123")
                Firstname = "Tutor",
                Lastname = "Teacher",
                RoleId = 2 // Tutor role
            }
        );

        // Seed data for profiles with static Guids
        var profileId1 = new Guid("d1e2f3a4-b5c6-7890-abcd-ef1234567890");
        var profileId2 = new Guid("e1f2a3b4-c5d6-7890-abcd-ef1234567890");
        var profileId3 = new Guid("febfaa10-358c-7890-abcd-ef1234567890");
        var profileId4 = new Guid("9eb76b25-ce6d-7890-abcd-ef1234567890");

        modelBuilder.Entity<Profile>().HasData(
            new Profile
            {
                Id = profileId1,
                UserId = userId1,
                Address = "123 Main St",
                Phone = "555-0123",
                City = "New York",
                State = "NY",
                ZipCode = "10001",
                Country = "USA",
                Nationality = "American",
                Bio = "A passionate student.",
                FacebookUrl = "https://facebook.com/johndoe",
                TwitterUrl = "https://twitter.com/johndoe"
            },
            new Profile
            {
                Id = profileId2,
                UserId = userId2,
                Address = "456 Oak St",
                Phone = "555-0456",
                City = "Los Angeles",
                State = "CA",
                ZipCode = "90001",
                Country = "USA",
                Nationality = "American",
                Bio = "An enthusiastic learner.",
                LinkedInUrl = "https://linkedin.com/in/janesmith"
            },
            new Profile
            {
                Id = profileId3,
                UserId = userId3,
                Address = "789 Admin Rd",
                Phone = "555-0789",
                City = "Chicago",
                State = "IL",
                ZipCode = "60601",
                Country = "USA",
                Nationality = "American",
                Bio = "An experienced administrator.",
                LinkedInUrl = "https://linkedin.com/in/adminuser"
            },
            new Profile
            {
                Id = profileId4,
                UserId = userId4,
                Address = "101 Tutor Ln",
                Phone = "555-1011",
                City = "Seattle",
                State = "WA",
                ZipCode = "98101",
                Country = "USA",
                Nationality = "American",
                Bio = "A dedicated tutor.",
                TwitterUrl = "https://twitter.com/tutorteacher"
            }
        );
    }
}