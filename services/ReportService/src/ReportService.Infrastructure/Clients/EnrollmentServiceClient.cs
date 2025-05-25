using ReportService.Application.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReportService.Infrastructure.Clients;

public class EnrollmentServiceClient : IEnrollmentServiceClient
{
    private readonly HttpClient _httpClient;

    public EnrollmentServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.BaseAddress = new Uri("http://localhost:5004/"); // EnrollmentService URL
    }

    public async Task<bool> CheckEnrollmentAsync(Guid studentId, Guid courseId)
    {
        var response = await _httpClient.GetAsync($"api/s2s/enrollment/check?studentId={studentId}&courseId={courseId}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return bool.TryParse(content, out var isEnrolled) && isEnrolled;
        }
        return false;
    }
}