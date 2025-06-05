using ReportService.Application.Interfaces;
using ReportService.Application.Configuration;
using ReportService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ReportService.Infrastructure.Clients;

public class EnrollmentServiceClient : IEnrollmentServiceClient
{
    private readonly HttpClient _httpClient; // For EnrollmentService requests
    private readonly HttpClient _auth0HttpClient; // For Auth0 token requests
    private readonly Auth0Settings _auth0Settings;

    public EnrollmentServiceClient(HttpClient httpClient, IOptions<Auth0Settings> auth0Settings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _auth0Settings = auth0Settings.Value ?? throw new ArgumentNullException(nameof(auth0Settings));

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
            return tokenData?.AccessToken ?? throw new Exception("Access token is null in response.");
        }

        throw new Exception($"Failed to obtain Auth0 OAuth2 token: {tokenResponse.StatusCode} - {await tokenResponse.Content.ReadAsStringAsync()}");
    }

    public async Task<bool> CheckEnrollmentAsync(Guid studentId, Guid courseId)
    {
        var accessToken = await GetAuth0OAuth2TokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/s2s/enrollments/check?studentId={studentId}&courseId={courseId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return bool.TryParse(content, out var isEnrolled) && isEnrolled;
        }
        return false;
    }
}