using AdminService.Application.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using AdminService.Application.Configuration;
using AdminService.Application.DTOs;
using Microsoft.Extensions.Options;
using System;

namespace AdminService.Infrastructure.Clients;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient; // For UserService requests
    private readonly HttpClient _auth0HttpClient; // For Auth0 token requests
    private readonly Auth0Settings _auth0Settings;

    public UserServiceClient(HttpClient httpClient, IOptions<Auth0Settings> auth0Settings)
    {
        _httpClient = httpClient;
        _auth0Settings = auth0Settings.Value;

        // Create a separate HttpClient for Auth0 token requests
        _auth0HttpClient = new HttpClient();
    }

    // Get OAuth2 token from Auth0
    private async Task<string> GetAuth0OAuth2TokenAsync()
    {
        var tokenRequest = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", _auth0Settings.ClientId },
            { "client_secret", _auth0Settings.ClientSecret },
            { "audience", _auth0Settings.Audience }
        };

        // Use the separate HttpClient for Auth0 requests
        var tokenResponse = await _auth0HttpClient.PostAsync(_auth0Settings.TokenUrl, new FormUrlEncodedContent(tokenRequest));

        if (tokenResponse.IsSuccessStatusCode)
        {
            var tokenData = JsonConvert.DeserializeObject<TokenResponse>(await tokenResponse.Content.ReadAsStringAsync());
            return tokenData.AccessToken;
        }

        throw new Exception($"Failed to obtain Auth0 OAuth2 token: {tokenResponse.StatusCode} - {await tokenResponse.Content.ReadAsStringAsync()}");
    }

    // Method to create user, with requests sent to the UserService using the token
    public async Task<Guid> CreateUserAsync(UserDTO user)
    {
        var accessToken = await GetAuth0OAuth2TokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Post, "api/s2s/users/register")
        {
            Content = new StringContent(JsonConvert.SerializeObject(user), System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Guid>(content);
        }
        throw new Exception($"Failed to create user: {response.StatusCode}");
    }

    public async Task<IEnumerable<UserDTO>> GetUsersByRoleAsync(string role)
    {
        var accessToken = await GetAuth0OAuth2TokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/s2s/users/by-role?role={role}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();
        }

        return null;
    }
}