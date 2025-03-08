using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserService.Domain.Entities;

// User model
public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }

    public int RoleId { get; set; }  // Foreign Key to Role
    public Role Role { get; set; }    // Navigation property

    public Profile Profile { get; set; }  // Navigation property to Profile
}
