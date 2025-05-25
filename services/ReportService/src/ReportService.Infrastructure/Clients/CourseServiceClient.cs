using ReportService.Application.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReportService.Infrastructure.Clients;

public class CourseServiceClient : ICourseServiceClient
{
    private readonly HttpClient _httpClient;

    public CourseServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.BaseAddress = new Uri("http://localhost:5000/"); // CourseService URL
    }

    public async Task<(string Title, int Credits)?> GetCourseDetailsAsync(Guid courseId)
    {
        var response = await _httpClient.GetAsync($"api/s2s/{courseId}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var course = JsonConvert.DeserializeObject<CourseDTO>(content);
            return (course?.Title, course?.Credits ?? 0);
        }
        return null;
    }
}

public class CourseDTO
{
    public string Title { get; set; }
    public int Credits { get; set; }
}