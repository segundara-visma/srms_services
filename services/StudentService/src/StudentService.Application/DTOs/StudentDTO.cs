namespace StudentService.Application.DTOs;

public record StudentDTO(Guid Id, Guid UserId, string FirstName, string LastName, string Email, string Role, ProfileDTO? Profile);