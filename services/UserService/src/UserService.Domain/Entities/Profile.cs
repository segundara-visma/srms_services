namespace UserService.Domain.Entities;

public class Profile
{
    public Guid Id { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    public string? Nationality { get; set; }
    public string? Bio { get; set; }

    // Social Media Links
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? WebsiteUrl { get; set; }

    // Foreign Key to User
    public Guid UserId { get; set; }
}