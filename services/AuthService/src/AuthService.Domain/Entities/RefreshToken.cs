namespace AuthService.Domain.Entities;

// RefreshToken model
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }  // Foreign key to User
    public required string Token { get; set; }  // The refresh token string
    public DateTime ExpiresAt { get; set; }  // Expiration date for the refresh token
    public bool IsActive { get; set; }  // Flag to indicate if the token is still valid
}