namespace AuthService.Application.DTOs;

public class LoginResponse
{
    public required string Token { get; set; }
    public Guid Id { get; set; }
}
