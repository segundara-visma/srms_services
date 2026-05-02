using System.Net.Http.Headers;
using System.Net.Http.Json;
using ApiGateway.DTOs;
using Newtonsoft.Json;

namespace ApiGateway.Services;

/// <summary>
/// Service responsible for aggregating tutor and courses details across multiple microservices.
/// Combines User,Tutor and Course data into a single response.
/// </summary>
public class TutorCoursesAggregationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    private readonly string _tutorUrl;
    private readonly string _userUrl;
    private readonly string _courseUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="TutorCoursesAggregationService"/> class.
    /// </summary>
    public TutorCoursesAggregationService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;

        // Fail fast if config is missing
        _tutorUrl = _config["Services:Tutor"]
            ?? throw new InvalidOperationException("Tutor service URL not configured");

        _userUrl = _config["Services:User"]
            ?? throw new InvalidOperationException("User service URL not configured");

        _courseUrl = _config["Services:Course"]
            ?? throw new InvalidOperationException("Course service URL not configured");
    }

    /// <summary>
    /// Retrieves courses for a specific tutor with full aggregated details.
    /// </summary>
    public async Task<PaginatedResponse<TutorCoursesFullDTO>> GetByTutorIdAsync(
        Guid userId, string token, int page, int pageSize)
    {
        var tutorCourses = await FetchTutorCoursesAsync(
            $"{_tutorUrl}/api/tutors/{userId}/courses?page={page}&pageSize={pageSize}",
            token);

        return await BuildFullResponse(tutorCourses, token);
    }

    /// <summary>
    /// Fetches enrollments from the Enrollment service.
    /// </summary>
    private async Task<PaginatedResponse<TutorCoursesDTO>> FetchTutorCoursesAsync(string url, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Tutor service failed: {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<PaginatedResponse<TutorCoursesDTO>>(content)
               ?? new PaginatedResponse<TutorCoursesDTO>();
    }

    /// <summary>
    /// Builds requests that has Bearer token included.
    /// </summary>
    private async Task<T?> GetWithAuthAsync<T>(string url, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return default;

        return await response.Content.ReadFromJsonAsync<T>();
    }

    /// <summary>
    /// Builds the full aggregated tutorCourses response.
    /// </summary>
    private async Task<PaginatedResponse<TutorCoursesFullDTO>> BuildFullResponse(
        PaginatedResponse<TutorCoursesDTO> tutorCourses,
        string token)
    {
        if (tutorCourses.Items == null || !tutorCourses.Items.Any())
        {
            return new PaginatedResponse<TutorCoursesFullDTO>
            {
                Items = [],
                Page = tutorCourses.Page,
                PageSize = tutorCourses.PageSize,
                TotalCount = tutorCourses.TotalCount
            };
        }

        //var studentIds = enrollments.Items.Select(e => e.StudentId).Distinct().ToList();
        var courseIds = tutorCourses.Items.Select(e => e.CourseId).Distinct().ToList();

        var courses = await GetWithAuthAsync<List<CourseDTO>>(
            $"{_courseUrl}/api/courses/batch?" + string.Join("&", courseIds.Select(id => $"ids={id}")),
            token
        ) ?? [];

        Console.WriteLine($"Course IDs requested: {string.Join(",", courseIds)}");
        Console.WriteLine($"Courses returned: {courses.Count}");

        var courseDict = courses.ToDictionary(c => c.Id);

        var items = tutorCourses.Items.Select(e =>
        {
            courseDict.TryGetValue(e.CourseId, out var course);

            Console.WriteLine($"Looking for: {e.CourseId}, Found: {course != null}");

            return new TutorCoursesFullDTO(
                e.Id,
                e.CourseId,
                course?.Name ?? "Unknown",
                course?.Code
            );
        }).ToList();

        return new PaginatedResponse<TutorCoursesFullDTO>
        {
            Items = items,
            Page = tutorCourses.Page,
            PageSize = tutorCourses.PageSize,
            TotalCount = tutorCourses.TotalCount
        };
    }
}