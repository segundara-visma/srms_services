namespace UserService.Application.DTOs;

public class RegisterResponse
{
    public Guid UserId { get; set; }
    public required string Email { get; set; }
}
