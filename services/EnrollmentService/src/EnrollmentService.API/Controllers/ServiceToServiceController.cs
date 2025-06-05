using EnrollmentService.Application.DTOs;
using EnrollmentService.Application.Services;
using EnrollmentService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EnrollmentService.API.Controllers;

/// <summary>
/// Controller for handling service-to-service API operations, restricted to Auth0-authenticated requests.
/// </summary>
[Route("api/s2s/enrollments")]
[ApiController]
[Authorize(AuthenticationSchemes = "Auth0")] // Restrict to Auth0-authenticated service requests
public class ServiceToServiceController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceToServiceController"/> class.
    /// </summary>
    /// <param name="enrollmentService">The course service.</param>
    public ServiceToServiceController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Checks if a student is enrolled in a specific course.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student (user ID).</param>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <returns>An <see cref="IActionResult"/> indicating whether the student is enrolled in the course (true/false).</returns>
    /// <response code="200">Returns true if the student is enrolled in the course, false otherwise.</response>
    [HttpGet("check")]
    public async Task<IActionResult> CheckEnrollment([FromQuery] Guid studentId, [FromQuery] Guid courseId)
    {
        var isEnrolled = await _enrollmentService.CheckEnrollmentAsync(studentId, courseId);
        return Ok(isEnrolled.ToString().ToLower());
    }

    /// <summary>
    /// Retrieves a list of all enrollments in the system.
    /// </summary>
    /// <returns>A list of <see cref="EnrollmentDTO"/> objects.</returns>
    /// <response code="200">Returns all enrollments successfully.</response>
    [HttpGet]
    public async Task<IActionResult> GetEnrollments()
    {
        var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
        return Ok(enrollments);
    }
}
