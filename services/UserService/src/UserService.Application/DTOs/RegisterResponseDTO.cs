namespace UserService.Application.DTOs;

public record RegisterResponseDTO(Guid UserId, string Email);

//public class RegisterResponse
//{
//    public Guid UserId { get; set; }
//    public required string Email { get; set; }
//}
