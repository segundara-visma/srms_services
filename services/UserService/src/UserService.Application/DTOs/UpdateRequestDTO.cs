namespace UserService.Application.DTOs;

public record UpdateRequestDTO(
    Guid Id,
    string? Email,
    string? Firstname,
    string? Lastname,
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

//public class UpdateRequest
//{
//    public required Guid Id { get; set; }
//    public string? Email { get; set; }
//    public string? Firstname { get; set; }
//    public string? Lastname { get; set; }
//    public string? Address { get; set; }
//    public string? Phone { get; set; }
//    public string? City { get; set; }
//    public string? State { get; set; }
//    public string? ZipCode { get; set; }
//    public string? Country { get; set; }
//    public string? Nationality { get; set; }
//    public string? Bio { get; set; }
//    public string? FacebookUrl { get; set; }
//    public string? TwitterUrl { get; set; }
//    public string? LinkedInUrl { get; set; }
//    public string? InstagramUrl { get; set; }
//    public string? WebsiteUrl { get; set; }
//}
