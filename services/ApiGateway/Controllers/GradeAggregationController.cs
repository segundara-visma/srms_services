using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiGateway.Services;
using ApiGateway.Extensions;
using System.Security.Claims;

namespace ApiGateway.Controllers;

/// <summary>
/// Provides aggregated endpoints for grade-related data.
/// Combines data from Grade, Student, Course, and User services
/// into a single response for frontend consumption.
/// </summary>
[ApiController]
[Route("gateway/aggregated/grades")]
[Authorize]
public class GradeAggregationController : ControllerBase
{
    private readonly GradeAggregationService _aggregationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GradeAggregationController"/> class.
    /// </summary>
    /// <param name="aggregationService">
    /// Service responsible for aggregating grade-related data across microservices.
    /// </param>
    public GradeAggregationController(GradeAggregationService aggregationService)
    {
        _aggregationService = aggregationService;
    }

    /// <summary>
    /// Retrieves all grades for the currently authenticated student.
    /// </summary>
    /// <remarks>
    /// This endpoint extracts the student/user identifier from the JWT token
    /// and returns enriched grade data including course and student details.
    /// </remarks>
    /// <returns>
    /// A list of aggregated grade records for the authenticated student.
    /// </returns>
    /// <response code="200">Returns the list of grades.</response>
    /// <response code="401">Unauthorized if the user is not authenticated.</response>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyGrades(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userRole = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

        if (string.IsNullOrEmpty(userRole) || userRole != "Student")
            return Unauthorized("User must be a student.");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token.");

        var token = HttpContext.GetBearerToken();
        if (token is null)
            return Unauthorized("No token provided.");

        var result = await _aggregationService
            .GetByStudentIdAsync(Guid.Parse(userId), token, page, pageSize);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all grades for a specific student.
    /// </summary>
    /// <param name="userId">The unique identifier of the student.</param>
    /// <returns>
    /// A list of aggregated grade records for the specified student.
    /// </returns>
    /// <response code="200">Returns the list of grades.</response>
    /// <response code="400">Invalid student ID.</response>
    [HttpGet("student/{userId:guid}")]
    public async Task<IActionResult> GetByStudentId(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var token = HttpContext.GetBearerToken();
        if (token is null)
            return Unauthorized("No token provided.");

        var result = await _aggregationService
            .GetByStudentIdAsync(userId, token, page, pageSize);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all grades for a specific course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <returns>
    /// A list of aggregated grade records for the specified course,
    /// including student and user details.
    /// </returns>
    /// <response code="200">Returns the list of grades.</response>
    /// <response code="400">Invalid course ID.</response>
    [HttpGet("course/{courseId:guid}")]
    public async Task<IActionResult> GetByCourseId(
        Guid courseId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var token = HttpContext.GetBearerToken();
        if (token is null)
            return Unauthorized("No token provided.");

        var result = await _aggregationService
            .GetByCourseIdAsync(courseId, token, page, pageSize);

        return Ok(result);
    }
}