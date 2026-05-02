using UserService.Domain.Entities;

namespace UserService.Application.DTOs;

public record UserResponseDTO(
    Guid Id,
    string Email,
    string Firstname,
    string Lastname,
    string Role,
    ProfileDTO? Profile
);

//public class UserResponse
//{
//    public Guid Id { get; set; }
//    public required string Email { get; set; }
//    public required string Firstname { get; set; }
//    public required string Lastname { get; set; }

//    public required string Role { get; set; }    // Navigation property

//    // Profile fields
//    public Profile? Profile { get; set; }
//}
