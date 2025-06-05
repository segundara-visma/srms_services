namespace AdminService.Application.DTOs;

public record UserDTO(Guid Id, string FirstName, string LastName, string Email, string Password, string Role);