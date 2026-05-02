using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiGateway.Services;
using ApiGateway.Extensions;
using System.Security.Claims; // Added to resolve ClaimTypes

namespace ApiGateway.Controllers;

/// <summary>
/// Provides aggregated endpoints for enrollment-related data.
/// Combines data from Enrollment, Student, Course, and User services
/// into a single response for frontend consumption.
/// </summary>
[ApiController]
[Route("gateway/aggregated/enrollments")]
[Authorize]
public class EnrollmentAggregationController : ControllerBase
{
    private readonly EnrollmentAggregationService _aggregationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrollmentAggregationController"/> class.
    /// </summary>
    /// <param name="aggregationService">
    /// Service responsible for aggregating enrollment-related data across microservices.
    /// </param>
    public EnrollmentAggregationController(EnrollmentAggregationService aggregationService)
    {
        _aggregationService = aggregationService;
    }

    /// <summary>
    /// Retrieves all enrollments for the currently authenticated student.
    /// </summary>
    /// <remarks>
    /// This endpoint extracts the student/user identifier from the JWT token
    /// and returns enriched enrollment data including course and student details.
    /// </remarks>
    /// <returns>
    /// A list of aggregated enrollment records for the authenticated student.
    /// </returns>
    /// <response code="200">Returns the list of enrollments.</response>
    /// <response code="401">Unauthorized if the user is not authenticated.</response>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyEnrollments(
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
    /// Retrieves all enrollments for a specific student.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student.</param>
    /// <returns>
    /// A list of aggregated enrollment records for the specified student.
    /// </returns>
    /// <response code="200">Returns the list of enrollments.</response>
    /// <response code="400">Invalid student ID.</response>
    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetByStudentId(
        Guid studentId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var token = HttpContext.GetBearerToken();
        if (token is null)
            return Unauthorized("No token provided.");

        var result = await _aggregationService
            .GetByStudentIdAsync(studentId, token, page, pageSize);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all enrollments for a specific course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <returns>
    /// A list of aggregated enrollment records for the specified course,
    /// including student and user details.
    /// </returns>
    /// <response code="200">Returns the list of enrollments.</response>
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