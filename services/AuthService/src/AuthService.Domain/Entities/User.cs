namespace AuthService.Domain.Entities;

// User model
public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
    public required string Role { get; set; }
}
