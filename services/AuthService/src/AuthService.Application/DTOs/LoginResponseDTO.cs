namespace AuthService.Application.DTOs;

public record LoginResponseDTO(
    string Token,
    Guid Id
);

//public class LoginResponse
//{
//    public required string Token { get; set; }
//    public Guid Id { get; set; }
//}
