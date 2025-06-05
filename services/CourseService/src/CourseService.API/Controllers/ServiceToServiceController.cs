using CourseService.Application.DTOs;
using CourseService.Application.Services;
using CourseService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CourseService.API.Controllers;

/// <summary>
/// Controller for handling service-to-service API operations, restricted to Auth0-authenticated requests.
/// </summary>
[Route("api/s2s/courses")]
[ApiController]
[Authorize(AuthenticationSchemes = "Auth0")] // Restrict to Auth0-authenticated service requests
public class ServiceToServiceController : ControllerBase
{
    private readonly ICourseService _courseService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceToServiceController"/> class.
    /// </summary>
    /// <param name="courseService">The course service.</param>
    public ServiceToServiceController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// StudentService requests retrieve a course by its ID.
    /// </summary>
    /// <param name="id">The ID of the course to retrieve.</param>
    /// <returns>The course details if found; otherwise, a 404 response.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> StudentServiceRequestCourseById(Guid id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        return course != null ? Ok(course) : NotFound();
    }

    /// <summary>
    /// Retrieves all courses.
    /// </summary>
    /// <returns>A list of all courses.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllCourses()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        return Ok(courses);
    }
}
