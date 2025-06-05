using AdminService.Application.Interfaces;
using AdminService.Application.Configuration;
using AdminService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace AdminService.Infrastructure.Clients;

public class GradeServiceClient : IGradeServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly HttpClient _auth0HttpClient;
    private readonly Auth0Settings _auth0Settings;

    public GradeServiceClient(HttpClient httpClient, IOptions<Auth0Settings> auth0Settings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _auth0Settings = auth0Settings.Value ?? throw new ArgumentNullException(nameof(auth0Settings));
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

        var tokenResponse = await _auth0HttpClient.PostAsync(_auth0Settings.TokenUrl, new FormUrlEncodedContent(tokenRequest));
        if (tokenResponse.IsSuccessStatusCode)
        {
            var tokenData = JsonConvert.DeserializeObject<TokenResponse>(await tokenResponse.Content.ReadAsStringAsync());
            return tokenData?.AccessToken ?? throw new Exception("Access token is null in response.");
        }
        throw new Exception($"Failed to obtain Auth0 OAuth2 token: {tokenResponse.StatusCode}");
    }

    public async Task<IEnumerable<GradeDTO>> GetAllGradesAsync()
    {
        var accessToken = await GetAuth0OAuth2TokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Get, "api/s2s/grades");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<GradeDTO>>(content);
        }
        throw new Exception($"Failed to get all grades: {response.StatusCode}");
    }
}