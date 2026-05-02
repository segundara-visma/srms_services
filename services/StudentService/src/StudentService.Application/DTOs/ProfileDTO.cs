namespace StudentService.Application.DTOs;

public record ProfileDTO(
    string? Address,
    string? Phone,
    string? City,
    string? State,
    string? ZipCode,
    string? Country,
    string? Nationality,
    string? Bio,
    string? FacebookUrl,
    string? TwitterUrl,
    string? LinkedInUrl,
    string? InstagramUrl,
    string? WebsiteUrl
);