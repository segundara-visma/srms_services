using Microsoft.AspNetCore.Mvc;
using GradeService.Application.Interfaces;
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
[Route("api/[controller]")]
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
    public async Task<IActionResult> CreateGrade([FromBody] GradeDTO gradeDto)
    {
        if (gradeDto == null) return BadRequest("Grade cannot be null.");

        try
        {
            await _gradeService.AddGradeAsync(gradeDto);
            return CreatedAtAction(nameof(GetGrade), new { id = gradeDto.Id }, gradeDto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message); // e.g., "Student is not enrolled in the specified course."
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); // e.g., "Grade value must be between 0 and 100."
        }
    }
}