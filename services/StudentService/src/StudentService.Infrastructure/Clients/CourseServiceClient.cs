using StudentService.Application.Interfaces;
using StudentService.Application.Configuration;
using StudentService.Application.Common;
using StudentService.Application.DTOs;

using Microsoft.Extensions.Options;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StudentService.Infrastructure.Clients;

public class CourseServiceClient : ICourseServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly HttpClient _auth0HttpClient;
    private readonly Auth0Settings _auth0Settings;

    public CourseServiceClient(HttpClient httpClient, IOptions<Auth0Settings> auth0Settings)
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

    public async Task<CourseDTO?> GetCourseByIdAsync(Guid courseId)
    {
        var accessToken = await GetAuth0OAuth2TokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/s2s/courses/{courseId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<CourseDTO>();
    }
}