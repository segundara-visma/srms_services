namespace TutorService.Application.DTOs;

public record UserDTO(Guid Id, string FirstName, string LastName, string Email, string Role);