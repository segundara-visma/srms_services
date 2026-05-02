namespace AuthService.Application.DTOs;

public record LoginRequestDTO(
    string Email,
    string Password
);

//public class LoginRequest
//{
//    public required string Email { get; set; }
//    public required string Password { get; set; }
//}
