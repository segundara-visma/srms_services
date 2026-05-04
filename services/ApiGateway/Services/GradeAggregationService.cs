using System.Net.Http.Headers;
using System.Net.Http.Json;
using ApiGateway.DTOs;
using Newtonsoft.Json;

namespace ApiGateway.Services;

/// <summary>
/// Service responsible for aggregating grade data across multiple microservices.
/// Combines Grade, Student, User, and Course data into a single response.
/// </summary>
public class GradeAggregationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    private readonly string _gradeUrl;
    private readonly string _studentUrl;
    private readonly string _userUrl;
    private readonly string _courseUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="GradeAggregationService"/> class.
    /// </summary>
    public GradeAggregationService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;

        _gradeUrl = _config["Services:Grade"]
            ?? throw new InvalidOperationException("Grade service URL not configured");

        _studentUrl = _config["Services:Student"]
            ?? throw new InvalidOperationException("Student service URL not configured");

        _userUrl = _config["Services:User"]
            ?? throw new InvalidOperationException("User service URL not configured");

        _courseUrl = _config["Services:Course"]
            ?? throw new InvalidOperationException("Course service URL not configured");
    }

    /// <summary>
    /// Retrieves grades for a specific student with full aggregated details.
    /// </summary>
    public async Task<PaginatedResponse<GradeFullDTO>> GetByStudentIdAsync(
        Guid userId, string token, int page, int pageSize)
    {
        var grades = await FetchGradesAsync(
            $"{_gradeUrl}/api/grades/student/{userId}?page={page}&pageSize={pageSize}",
            token);

        return await BuildFullResponse(grades, token);
    }

    /// <summary>
    /// Retrieves grades for a specific course with full aggregated details.
    /// </summary>
    public async Task<PaginatedResponse<GradeFullDTO>> GetByCourseIdAsync(
        Guid courseId, string token, int page, int pageSize)
    {
        var grades = await FetchGradesAsync(
            $"{_gradeUrl}/api/grades/course/{courseId}?page={page}&pageSize={pageSize}",
            token);

        return await BuildFullResponse(grades, token);
    }

    /// <summary>
    /// Fetches grades from the Grade service.
    /// </summary>
    private async Task<PaginatedResponse<GradeDTO>> FetchGradesAsync(string url, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Grade service failed: {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<PaginatedResponse<GradeDTO>>(content)
               ?? new PaginatedResponse<GradeDTO>();
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
    /// Builds the full aggregated grade response.
    /// </summary>
    private async Task<PaginatedResponse<GradeFullDTO>> BuildFullResponse(
        PaginatedResponse<GradeDTO> grades,
        string token)
    {
        if (grades.Items == null || !grades.Items.Any())
        {
            return new PaginatedResponse<GradeFullDTO>
            {
                Items = [],
                Page = grades.Page,
                PageSize = grades.PageSize,
                TotalCount = grades.TotalCount
            };
        }

        var studentIds = grades.Items.Select(g => g.StudentId).Distinct().ToList();
        var courseIds = grades.Items.Select(g => g.CourseId).Distinct().ToList();

        var studentsTask = GetWithAuthAsync<List<StudentDTO>>(
            $"{_studentUrl}/api/students/batch?" + string.Join("&", studentIds.Select(id => $"ids={id}")),
            token);

        var coursesTask = GetWithAuthAsync<List<CourseDTO>>(
            $"{_courseUrl}/api/courses/batch?" + string.Join("&", courseIds.Select(id => $"ids={id}")),
            token);

        await Task.WhenAll(studentsTask, coursesTask);

        var students = studentsTask.Result ?? [];
        var courses = coursesTask.Result ?? [];

        var userIds = students.Select(s => s.UserId).Distinct().ToList();

        var users = await GetWithAuthAsync<List<UserDTO>>(
            $"{_userUrl}/api/users/batch?" + string.Join("&", userIds.Select(id => $"ids={id}")),
            token
        ) ?? [];

        var studentDict = students.ToDictionary(s => s.Id);
        var userDict = users.ToDictionary(u => u.Id);
        var courseDict = courses.ToDictionary(c => c.Id);

        var items = grades.Items.Select(g =>
        {
            studentDict.TryGetValue(g.StudentId, out var student);
            userDict.TryGetValue(student?.UserId ?? Guid.Empty, out var user);
            courseDict.TryGetValue(g.CourseId, out var course);

            return new GradeFullDTO(
                g.Id,
                g.StudentId,
                user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                g.CourseId,
                course?.Name ?? "Unknown",
                g.GradeValue
            );
        }).ToList();

        return new PaginatedResponse<GradeFullDTO>
        {
            Items = items,
            Page = grades.Page,
            PageSize = grades.PageSize,
            TotalCount = grades.TotalCount
        };
    }
}