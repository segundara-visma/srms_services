using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using System.Net.Http;
using System.Net.Http.Json;

namespace AuthService.Application.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Method to make authenticated requests to the UserService using the token
    public async Task<User> GetUserByEmailAsync(string email)
    {
        // This method still needs to be implemented
        var user = new User
        {
            Id = new Guid(),
            Email = email,
            Firstname = "Firstname",
            Lastname = "Lastname",
            Role = "Student"
        };
        return user;
    }

    public async Task<bool> ValidatePasswordAsync(Guid userId, string password)
    {
        // This method still needs to be implemented
        return true;
    }
}
