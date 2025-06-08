using StudentService.Application.DTOs;
using StudentService.Application.Services;
using StudentService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Security.Claims; // Added to resolve ClaimTypes

namespace StudentService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Retrieves a student by their User ID.
    /// </summary>
    [HttpGet("{userId}")]
    [Authorize]
    public async Task<IActionResult> GetStudentById(Guid userId)
    {
        try
        {
            var student = await _studentService.GetStudentByIdAsync(userId);
            return Ok(student);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves the currently signed-in student.
    /// </summary>
    /// <returns>
    /// An object containing the student's details if found, or a 401 Not authorized.
    /// </returns>
    /// <response code="200">Returns the student details.</response>
    /// <response code="401">If the request is not authorized.</response>
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var student = await _studentService.GetStudentByIdAsync(Guid.Parse(userId));
            return Ok(student);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Updates the currently signed-in student.
    /// </summary>
    /// <returns>
    /// A 204 No Content response if the assignment is successful, a 400 Bad Request, or a 401 Not Authorize.
    /// </returns>
    /// <response code="204">Returns No Content response.</response>
    /// <response code="401">If the request is not authorized.</response>
    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateRequest dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            dto = dto with { Id = Guid.Parse(userId) }; // Set Id from token
            await _studentService.UpdateStudentAsync(Guid.Parse(userId), dto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all students.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminOrTutor")] // Either Admin or Tutor can access
    public async Task<IActionResult> GetAllStudents()
    {
        var students = await _studentService.GetAllStudentsAsync();
        return Ok(students);
    }

    ///// <summary>
    ///// Enrolls a student in a course.
    ///// </summary>
    //[HttpPost("{userId}/enroll")]
    //public async Task<IActionResult> EnrollStudent(Guid userId, [FromBody] EnrollStudentRequest request)
    //{
    //    try
    //    {
    //        await _studentService.EnrollStudentAsync(userId, request.CourseId);
    //        return NoContent();
    //    }
    //    catch (ArgumentException ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //    catch (InvalidOperationException ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}

    ///// <summary>
    ///// Retrieves all courses a student is enrolled in.
    ///// </summary>
    //[HttpGet("{userId}/courses")]
    //public async Task<IActionResult> GetStudentCourses(Guid userId)
    //{
    //    try
    //    {
    //        var courses = await _studentService.GetStudentCoursesAsync(userId);
    //        return Ok(courses);
    //    }
    //    catch (ArgumentException ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}
}

//public record EnrollStudentRequest(Guid CourseId);