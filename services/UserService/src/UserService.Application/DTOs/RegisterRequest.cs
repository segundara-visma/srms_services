namespace UserService.Application.DTOs;

public class RegisterRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
    public required string Role { get; set; }
}
