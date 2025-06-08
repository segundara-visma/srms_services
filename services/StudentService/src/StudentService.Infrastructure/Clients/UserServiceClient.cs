using StudentService.Application.Interfaces;
using StudentService.Domain.Entities;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using StudentService.Application.Configuration;
using StudentService.Application.DTOs;
using Microsoft.Extensions.Options;
using System;

namespace StudentService.Infrastructure.Clients;

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

    // Method to make requests to the UserService using the token
    public async Task<UserDTO> GetUserByIdAsync(Guid userId)
    {
        var accessToken = await GetAuth0OAuth2TokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/s2s/users/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserDTO>();
        }

        return null;
    }

    public async Task<UserDTO> UpdateUserAsync(Guid userId, UpdateRequest user)
    {
        var accessToken = await GetAuth0OAuth2TokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Put, $"api/s2s/users/{userId}")
        {
            Content = new StringContent(JsonConvert.SerializeObject(user), System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserDTO>();
        }
        throw new Exception($"Failed to update user: {response.StatusCode}");
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