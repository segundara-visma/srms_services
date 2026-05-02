using System.Net.Http.Headers;
using System.Net.Http.Json;
using ApiGateway.DTOs;
using Newtonsoft.Json;

namespace ApiGateway.Services;

/// <summary>
/// Service responsible for aggregating enrollment data across multiple microservices.
/// Combines Enrollment, Student, User, and Course data into a single response.
/// </summary>
public class EnrollmentAggregationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    private readonly string _enrollmentUrl;
    private readonly string _studentUrl;
    private readonly string _userUrl;
    private readonly string _courseUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrollmentAggregationService"/> class.
    /// </summary>
    public EnrollmentAggregationService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;

        // Fail fast if config is missing
        _enrollmentUrl = _config["Services:Enrollment"]
            ?? throw new InvalidOperationException("Enrollment service URL not configured");

        _studentUrl = _config["Services:Student"]
            ?? throw new InvalidOperationException("Student service URL not configured");

        _userUrl = _config["Services:User"]
            ?? throw new InvalidOperationException("User service URL not configured");

        _courseUrl = _config["Services:Course"]
            ?? throw new InvalidOperationException("Course service URL not configured");
    }

    /// <summary>
    /// Retrieves enrollments for a specific student with full aggregated details.
    /// </summary>
    public async Task<PaginatedResponse<EnrollmentFullDTO>> GetByStudentIdAsync(
        Guid studentId, string token, int page, int pageSize)
    {
        var enrollments = await FetchEnrollmentsAsync(
            $"{_enrollmentUrl}/api/enrollments/student/{studentId}?page={page}&pageSize={pageSize}",
            token);

        return await BuildFullResponse(enrollments, token);
    }

    /// <summary>
    /// Retrieves enrollments for a specific course with full aggregated details.
    /// </summary>
    public async Task<PaginatedResponse<EnrollmentFullDTO>> GetByCourseIdAsync(
        Guid courseId, string token, int page, int pageSize)
    {
        var enrollments = await FetchEnrollmentsAsync(
            $"{_enrollmentUrl}/api/enrollments/course/{courseId}?page={page}&pageSize={pageSize}",
            token);

        return await BuildFullResponse(enrollments, token);
    }

    /// <summary>
    /// Fetches enrollments from the Enrollment service.
    /// </summary>
    private async Task<PaginatedResponse<EnrollmentDTO>> FetchEnrollmentsAsync(string url, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Enrollment service failed: {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<PaginatedResponse<EnrollmentDTO>>(content)
               ?? new PaginatedResponse<EnrollmentDTO>();
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
    /// Builds the full aggregated enrollment response.
    /// </summary>
    private async Task<PaginatedResponse<EnrollmentFullDTO>> BuildFullResponse(
        PaginatedResponse<EnrollmentDTO> enrollments,
        string token)
    {
        if (enrollments.Items == null || !enrollments.Items.Any())
        {
            return new PaginatedResponse<EnrollmentFullDTO>
            {
                Items = [],
                Page = enrollments.Page,
                PageSize = enrollments.PageSize,
                TotalCount = enrollments.TotalCount
            };
        }

        var studentIds = enrollments.Items.Select(e => e.StudentId).Distinct().ToList();
        var courseIds = enrollments.Items.Select(e => e.CourseId).Distinct().ToList();

        var studentsTask = GetWithAuthAsync<List<StudentDTO>>(
            $"{_studentUrl}/api/students/batch?" + string.Join("&", studentIds.Select(id => $"ids={id}")),
            token);

        var coursesTask = GetWithAuthAsync<List<CourseDTO>>(
            $"{_courseUrl}/api/courses/batch?" + string.Join("&", courseIds.Select(id => $"ids={id}")),
            token);

        await Task.WhenAll(studentsTask, coursesTask);

        var students = studentsTask.Result ?? [];
        var courses = coursesTask.Result ?? [];

        Console.WriteLine($"Course IDs requested: {string.Join(",", courseIds)}");
        Console.WriteLine($"Courses returned: {courses.Count}");

        var userIds = students.Select(s => s.UserId).Distinct().ToList();

        var users = await GetWithAuthAsync<List<UserDTO>>(
            $"{_userUrl}/api/users/batch?" + string.Join("&", userIds.Select(id => $"ids={id}")),
            token
        ) ?? [];

        var studentDict = students.ToDictionary(s => s.Id);
        var userDict = users.ToDictionary(u => u.Id);
        var courseDict = courses.ToDictionary(c => c.Id);

        var items = enrollments.Items.Select(e =>
        {
            studentDict.TryGetValue(e.StudentId, out var student);
            userDict.TryGetValue(student?.UserId ?? Guid.Empty, out var user);
            courseDict.TryGetValue(e.CourseId, out var course);

            return new EnrollmentFullDTO(
                e.Id,
                e.StudentId,
                user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                e.CourseId,
                course?.Name ?? "Unknown",
                e.Status
            );
        }).ToList();

        return new PaginatedResponse<EnrollmentFullDTO>
        {
            Items = items,
            Page = enrollments.Page,
            PageSize = enrollments.PageSize,
            TotalCount = enrollments.TotalCount
        };
    }
}