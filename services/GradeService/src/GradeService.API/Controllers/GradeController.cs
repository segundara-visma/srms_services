using Microsoft.AspNetCore.Mvc;
using GradeService.Application.Interfaces;
using GradeService.Application.Common;
using GradeService.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace GradeService.API.Controllers;

/// <summary>
/// Controller for managing grade-related operations in the GradeService.
/// Provides endpoints to retrieve, create, and manage grades for students.
/// </summary>
[ApiController]
[Route("api/grades")]
public class GradeController : ControllerBase
{
    private readonly IGradeService _gradeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GradeController"/> class.
    /// </summary>
    /// <param name="gradeService">The service responsible for grade operations.</param>
    public GradeController(IGradeService gradeService)
    {
        _gradeService = gradeService ?? throw new ArgumentNullException(nameof(gradeService));
    }

    /// <summary>
    /// Retrieves a specific grade by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the grade.</param>
    /// <returns>The <see cref="GradeDTO"/> object if found; otherwise, a 404 Not Found response.</returns>
    /// <response code="200">Returns the requested grade.</response>
    /// <response code="404">If the grade with the specified ID is not found.</response>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetGrade(Guid id)
    {
        var grade = await _gradeService.GetGradeByIdAsync(id);
        if (grade == null) return NotFound();
        return Ok(grade);
    }

    /// <summary>
    /// Creates a new grade for a student in a specific course.
    /// Validates that the student is enrolled in the course before saving the grade.
    /// </summary>
    /// <param name="gradeDto">The grade details to create, including StudentId, CourseId, GradeValue, DateGraded, and optional Comments.</param>
    /// <returns>The created <see cref="GradeDTO"/> object with a 201 Created status.</returns>
    /// <response code="201">Returns the newly created grade.</response>
    /// <response code="400">If the grade object is null or invalid.</response>
    /// <response code="409">If the student is not enrolled in the specified course.</response>
    [HttpPost]
    [Authorize(Policy = "TutorOnly")]
    public async Task<IActionResult> CreateGrade([FromBody] CreateGradeDTO gradeDto)
    {
        if (gradeDto == null)
            return BadRequest("Grade cannot be null.");

        try
        {
            var createdId = await _gradeService.AddGradeAsync(gradeDto);

            return CreatedAtAction(
                nameof(GetGrade),
                new { id = createdId },
                null // or return DTO if you want
            );
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves all grades for a specific student.
    /// </summary>
    /// <param name="userId">The unique identifier of the user (student).</param>
    /// <param name="page">The page number (default: 1).</param>
    /// <param name="pageSize">The number of items per page (default: 10).</param>
    /// <returns>
    /// A <see cref="PaginatedResponse{GradeDTO}"/> containing the paginated list of grades for the selected student.
    /// </returns>
    /// <response code="200">Returns the paginated list of grades.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpGet("student/{userId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetGradesByStudent(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var grades = await _gradeService.GetGradesByStudentAsync(userId, page, pageSize);
            return Ok(grades);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all grades for a specific course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="page">The page number (default: 1).</param>
    /// <param name="pageSize">The number of items per page (default: 10).</param>
    /// <returns>
    /// A <see cref="PaginatedResponse{GradeDTO}"/> containing the paginated list of grades for the selected course.
    /// </returns>
    /// <response code="200">Returns the paginated list of grades.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpGet("course/{courseId}")]
    [Authorize]
    public async Task<IActionResult> GetGradesByCourse(
        Guid courseId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var grades = await _gradeService.GetGradesByCourseAsync(courseId, page, pageSize);
            return Ok(grades);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}