using CourseService.Application.DTOs;
using CourseService.Application.Services;
using CourseService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CourseService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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