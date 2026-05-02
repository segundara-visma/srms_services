namespace TutorService.Application.DTOs;

public record TutorDTO(Guid Id, Guid UserId, string FirstName, string LastName, string Email, string Role, ProfileDTO? Profile);
