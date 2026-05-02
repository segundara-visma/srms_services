using CourseService.Application.DTOs;
using CourseService.Application.Services;
using CourseService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CourseService.API.Controllers;

[Route("api/courses")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// Creates a new course.
    /// </summary>
    /// <param name="dto">The course details to create.</param>
    /// <returns>The ID of the created course.</returns>
    [HttpPost]
    [Authorize(Policy = "AdminOrTutor")] // Either Admin or Tutor can access
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDTO dto)
    {
        var courseId = await _courseService.CreateCourseAsync(dto);
        return CreatedAtAction(nameof(GetCourseById), new { id = courseId }, new { Id = courseId });
    }

    /// <summary>
    /// Retrieves a course by its ID.
    /// </summary>
    /// <param name="id">The ID of the course to retrieve.</param>
    /// <returns>The course details if found; otherwise, a 404 response.</returns>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetCourseById(Guid id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        return course != null ? Ok(course) : NotFound();
    }

    /// <summary>
    /// Updates an existing course.
    /// </summary>
    /// <param name="id">The ID of the course to update.</param>
    /// <param name="dto">The updated course details.</param>
    /// <returns>A 204 response if successful; otherwise, an error message.</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOrTutor")] // Either Admin or Tutor can access
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDTO dto)
    {
        try
        {
            await _courseService.UpdateCourseAsync(id, dto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves courses in batch by IDs.
    /// </summary>
    [HttpGet("batch")]
    [Authorize]
    public async Task<IActionResult> GetCoursesByIds([FromQuery] List<Guid> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest("Ids are required");

        var courses = await _courseService.GetByIdsAsync(ids);
        return Ok(courses);
    }

    /// <summary>
    /// Retrieves all courses.
    /// </summary>
    /// <param name="page">The page number (default: 1).</param>
    /// <param name="pageSize">The number of items per page (default: 10).</param>
    /// <returns>
    /// A <see cref="PaginatedResponse{CourseDTO}"/> containing the paginated list of courses.
    /// </returns>
    /// <response code="200">Returns the paginated list of courses.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllCourses([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var courses = await _courseService.GetAllCoursesWithPaginationAsync(page, pageSize);
            return Ok(courses);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}