using StudentService.Application.Interfaces;
using StudentService.Application.Configuration;
using StudentService.Application.Common;
using StudentService.Application.DTOs;

using Microsoft.Extensions.Options;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace StudentService.Infrastructure.Clients;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly HttpClient _auth0HttpClient;
    private readonly Auth0Settings _auth0Settings;

    public UserServiceClient(HttpClient httpClient, IOptions<Auth0Settings> auth0Settings)
    {
        _httpClient = httpClient;
        _auth0Settings = auth0Settings.Value;
        _auth0HttpClient = new HttpClient();
    }

    private async Task<string> GetAuth0OAuth2TokenAsync()
    {
        var tokenRequest = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", _auth0Settings.ClientId },
            { "client_secret", _auth0Settings.ClientSecret },
            { "audience", _auth0Settings.Audience }
        };

        var response = await _auth0HttpClient.PostAsync(
            _auth0Settings.TokenUrl,
            new FormUrlEncodedContent(tokenRequest));

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Auth0 token failed: {response.StatusCode} - {content}");

        var tokenData = JsonConvert.DeserializeObject<TokenResponse>(content);

        if (tokenData?.AccessToken == null)
            throw new Exception("Auth0 returned invalid token response.");

        return tokenData.AccessToken;
    }

    public async Task<UserDTO?> GetUserByIdAsync(Guid userId)
    {
        var accessToken = await GetAuth0OAuth2TokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/s2s/users/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UserDTO>();
    }

    public async Task<UserDTO?> UpdateUserAsync(Guid userId, UpdateRequestDTO user)
    {
        var accessToken = await GetAuth0OAuth2TokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Put, $"api/s2s/users/{userId}")
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to update user: {response.StatusCode}");

        return await response.Content.ReadFromJsonAsync<UserDTO>();
    }

    public async Task<PaginatedResponse<UserDTO>> GetUsersByRoleAsync(string role, int page = 1, int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty.", nameof(role));

        var accessToken = await GetAuth0OAuth2TokenAsync();

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/s2s/users/by-role?role={Uri.EscapeDataString(role)}&page={page}&pageSize={pageSize}");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return new PaginatedResponse<UserDTO>
            {
                Items = Enumerable.Empty<UserDTO>(),
                TotalCount = 0,
                Page = page,
                PageSize = pageSize
            };
        }

        var result =
            await response.Content.ReadFromJsonAsync<PaginatedResponse<UserDTO>>();

        return result ?? new PaginatedResponse<UserDTO>
        {
            Items = Enumerable.Empty<UserDTO>(),
            TotalCount = 0,
            Page = page,
            PageSize = pageSize
        };
    }
}