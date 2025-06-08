namespace AdminService.Application.DTOs;

public record TutorDTO(Guid Id, Guid UserId, string FirstName, string LastName, string Email);
