namespace ApiGateway.DTOs;

public record UserDTO(
    Guid Id,
    string FirstName,
    string LastName
);