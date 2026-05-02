using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiGateway.Services;
using ApiGateway.Extensions;
using System.Security.Claims; // Added to resolve ClaimTypes

namespace ApiGateway.Controllers;

/// <summary>
/// Provides aggregated endpoints for all courses assigned to a tutor
/// </summary>
[ApiController]
[Route("gateway/aggregated/tutors")]
[Authorize]
public class TutorCoursesAggregationController : ControllerBase
{
    private readonly TutorCoursesAggregationService _aggregationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TutorCoursesAggregationController"/> class.
    /// </summary>
    /// <param name="aggregationService">
    /// Service responsible for aggregating data across microservices.
    /// </param>
    public TutorCoursesAggregationController(TutorCoursesAggregationService aggregationService)
    {
        _aggregationService = aggregationService;
    }

    /// <summary>
    /// Retrieves all courses for the currently authenticated tutor.
    /// </summary>
    /// <remarks>
    /// This endpoint extracts the tutor/user identifier from the JWT token
    /// and returns courses details.
    /// </remarks>
    /// <returns>
    /// A list of aggregated courses for the authenticated tutor.
    /// </returns>
    /// <response code="200">Returns the list of courses.</response>
    /// <response code="401">Unauthorized if the user is not authenticated.</response>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyCourses(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userRole = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

        if (string.IsNullOrEmpty(userRole) || userRole != "Tutor")
            return Unauthorized("User must be a tutor.");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token.");

        var token = HttpContext.GetBearerToken();
        if (token is null)
            return Unauthorized("No token provided.");

        var result = await _aggregationService
            .GetByTutorIdAsync(Guid.Parse(userId), token, page, pageSize);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all courses assigned to a specific tutor.
    /// </summary>
    /// <param name="userId">The unique identifier of the tutor.</param>
    /// <returns>
    /// A list of aggregated courses for the specified tutor.
    /// </returns>
    /// <response code="200">Returns the list of courses.</response>
    /// <response code="400">Invalid tutor ID.</response>
    [HttpGet("{userId:guid}/courses")]
    public async Task<IActionResult> GetByTutorId(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var token = HttpContext.GetBearerToken();
        if (token is null)
            return Unauthorized("No token provided.");

        var result = await _aggregationService
            .GetByTutorIdAsync(userId, token, page, pageSize);

        return Ok(result);
    }
}