using StudentService.Application.Common;
using StudentService.Application.DTOs;
using StudentService.Application.Services;
using StudentService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Security.Claims; // Added to resolve ClaimTypes

namespace StudentService.API.Controllers;

[Route("api/students")]
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
    [Authorize(Roles = "Student")]
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
    [Authorize(Roles = "Student")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateRequestDTO dto)
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
    /// <param name="page">The page number (default: 1).</param>
    /// <param name="pageSize">The number of items per page (default: 10).</param>
    /// <returns>
    /// A <see cref="PaginatedResponse{StudentDTO}"/> containing the paginated list of users.
    /// </returns>
    /// <response code="200">Returns the paginated list of users.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpGet]
    [Authorize(Policy = "AdminOrTutor")] // Either Admin or Tutor can access
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllStudents([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        //var students = await _studentService.GetAllStudentsAsync();
        //return Ok(students);
        try
        {
            var students = await _studentService.GetAllStudentsAsync(page, pageSize);
            return Ok(students);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves students in batch by IDs.
    /// </summary>
    [HttpGet("batch")]
    [Authorize]
    public async Task<IActionResult> GetStudentsByIds([FromQuery] List<Guid> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest("Ids are required");

        var students = await _studentService.GetByIdsAsync(ids);
        return Ok(students);
    }
}