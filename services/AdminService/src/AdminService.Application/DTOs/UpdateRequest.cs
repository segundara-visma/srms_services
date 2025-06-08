namespace AdminService.Application.DTOs;

public record UpdateRequest(Guid Id, string? FirstName, string? LastName, string? Email,
    string? Address, string? Phone, string? City, string? State, string? ZipCode,
    string? Country, string? Nationality, string? Bio, string? FacebookUrl,
    string? TwitterUrl, string? LinkedInUrl, string? InstagramUrl, string? WebsiteUrl);

