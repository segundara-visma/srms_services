using Microsoft.AspNetCore.Mvc;
using GradeService.Application.Interfaces;
using GradeService.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace GradeService.API.Controllers;

/// <summary>
/// Controller for handling service-to-service API operations, restricted to Auth0-authenticated requests.
/// </summary>
[ApiController]
[Route("api/s2s/grades")]
[Authorize(AuthenticationSchemes = "Auth0")] // Restrict to Auth0-authenticated service requests
public class ServiceToServiceController : ControllerBase
{
    private readonly IGradeService _gradeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceToServiceController"/> class.
    /// </summary>
    /// <param name="gradeService">The course service.</param>
    public ServiceToServiceController(IGradeService gradeService)
    {
        _gradeService = gradeService ?? throw new ArgumentNullException(nameof(gradeService));
    }

    /// <summary>
    /// StudentService requests retrieve grade by studentId.
    /// </summary>
    /// <param name="studentId">The ID of the student whose grades are to be retrieved.</param>
    /// <returns>The grades details if found.</returns>
    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetGradesByStudent(Guid studentId)
    {
        var grades = await _gradeService.GetGradesByStudentAsync(studentId);
        return Ok(grades);
    }

    /// <summary>
    /// Assigns a grade to a student for a specific course, requested by TutorService.
    /// </summary>
    /// <param name="request">The request object containing the student ID, course ID, and grade value.</param>
    /// <returns>
    /// A 204 No Content response if the grade is successfully assigned, a 400 Bad Request if the input is invalid, or a 403 Forbidden if the student is not enrolled.
    /// </returns>
    /// <response code="204">Grade was successfully assigned.</response>
    /// <response code="400">If the grade value is invalid (e.g., not between 0 and 100).</response>
    /// <response code="403">If the student is not enrolled in the specified course.</response>
    [HttpPost("assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignGrade([FromBody] AssignGradeRequest request)
    {
        try
        {
            await _gradeService.AssignGradeAsync(request.StudentId, request.CourseId, request.Grade);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a list of all grades recorded in the system.
    /// </summary>
    /// <returns>A list of <see cref="GradeDTO"/> objects.</returns>
    /// <response code="200">Returns the list of grades successfully.</response>
    [HttpGet]
    public async Task<IActionResult> GetGrades()
    {
        var grades = await _gradeService.GetAllGradesAsync();
        return Ok(grades);
    }
}

/// <summary>
/// Represents the request payload for assigning a grade to a student.
/// </summary>
public class AssignGradeRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the student receiving the grade.
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the course for which the grade is assigned.
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Gets or sets the numerical grade value (must be between 0 and 100).
    /// </summary>
    public decimal Grade { get; set; }
}