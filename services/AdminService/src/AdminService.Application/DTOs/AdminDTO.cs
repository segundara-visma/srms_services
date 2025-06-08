namespace AdminService.Application.DTOs;

public record AdminDTO(Guid Id, string FirstName, string LastName, string Email, string Role, Profile? Profile);