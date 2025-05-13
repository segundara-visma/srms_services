namespace EnrollmentService.Application.Configuration;

public class Auth0Settings
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string TokenUrl { get; set; }
    public string Audience { get; set; }
}
