using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using AuthService.Application.Configuration;
using AuthService.Application.DTOs;
using Microsoft.Extensions.Options;

namespace AuthService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly Auth0Settings _auth0Settings;

        public UserService(HttpClient httpClient, IOptions<Auth0Settings> auth0Settings)
        {
            _httpClient = httpClient;
            _auth0Settings = auth0Settings.Value;  // Store Auth0Settings
        }

        // Get OAuth2 token from Okta
        private async Task<string> GetAuth0OAuth2TokenAsync()
        {
            var tokenRequest = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _auth0Settings.ClientId },  // Use injected ClientId (now for Auth0)
                { "client_secret", _auth0Settings.ClientSecret },  // Use injected ClientSecret (now for Auth0)
                { "audience", _auth0Settings.Audience }  // Audience is important for Auth0
            };

            var tokenResponse = await _httpClient.PostAsync(_auth0Settings.TokenUrl, new FormUrlEncodedContent(tokenRequest));

            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokenData = JsonConvert.DeserializeObject<TokenResponse>(await tokenResponse.Content.ReadAsStringAsync());
                return tokenData.AccessToken;
            }

            throw new Exception("Failed to obtain Auth0 OAuth2 token");
        }

        // Method to make authenticated requests to the UserService using the token
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var accessToken = await GetAuth0OAuth2TokenAsync();  // Get the OAuth2 token from Okta

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/authservice/by-email/{email}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }

            return null;
        }

        public async Task<bool> ValidatePasswordAsync(Guid userId, string password)
        {
            var accessToken = await GetAuth0OAuth2TokenAsync();  // Get the OAuth2 token from Okta

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/authservice/by-password/{userId}/{password}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}
