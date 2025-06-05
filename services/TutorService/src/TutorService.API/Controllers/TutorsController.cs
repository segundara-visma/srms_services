using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorService.Application.Interfaces;

/// <summary>
/// Provides RESTful API endpoints for managing tutor-related operations within the Student Record Management System (SRMS).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TutorsController : ControllerBase
{
    private readonly ITutorService _tutorService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TutorsController"/> class with the specified tutor service.
    /// </summary>
    /// <param name="tutorService">The service responsible for tutor-related business logic. Must not be null.</param>
    public TutorsController(ITutorService tutorService)
    {
        _tutorService = tutorService ?? throw new ArgumentNullException(nameof(tutorService));
    }

    /// <summary>
    /// Retrieves a tutor by their unique identifier.
    /// </summary>
    /// <param name="tutorId">The unique identifier of the tutor to retrieve.</param>
    /// <returns>
    /// A <see cref="TutorDTO"/> object containing the tutor's details if found, or a 404 Not Found response if the tutor does not exist.
    /// </returns>
    /// <response code="200">Returns the tutor details.</response>
    /// <response code="404">If the tutor with the specified ID is not found.</response>
    [HttpGet("{tutorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "AdminOrTutor")] // Either Admin or Tutor can access
    public async Task<ActionResult<TutorDTO>> GetTutorByIdAsync([FromRoute] Guid tutorId)
    {
        try
        {
            var tutor = await _tutorService.GetTutorByIdAsync(tutorId);
            return Ok(tutor);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Assigns a grade to a student for a specific course by a tutor.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student receiving the grade.</param>
    /// <param name="courseId">The unique identifier of the course for which the grade is assigned.</param>
    /// <param name="grade">The numerical grade value (must be between 0 and 100).</param>
    /// <returns>
    /// A boolean indicating success (<c>true</c>) or failure (<c>false</c>) of the grade assignment, or a 400 Bad Request if the input is invalid.
    /// </returns>
    /// <response code="200">Returns true if the grade was successfully assigned.</response>
    /// <response code="400">If the grade is invalid or other input constraints are violated.</response>
    [HttpPost("grades")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "TutorOnly")]
    public async Task<ActionResult<bool>> AssignGradeAsync(
        [FromQuery] Guid studentId,
        [FromQuery] Guid courseId,
        [FromQuery] decimal grade)
    {
        try
        {
            var result = await _tutorService.AssignGradeAsync(studentId, courseId, grade);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a list of course IDs assigned to a specific tutor.
    /// </summary>
    /// <param name="tutorId">The unique identifier of the tutor whose assigned courses are to be retrieved.</param>
    /// <returns>
    /// An <see cref="IEnumerable{Guid}"/> containing the IDs of courses assigned to the tutor, or a 404 Not Found response if the tutor does not exist.
    /// </returns>
    /// <response code="200">Returns a list of course IDs.</response>
    /// <response code="404">If the tutor with the specified ID is not found.</response>
    [HttpGet("{tutorId}/courses")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "AdminOrTutor")] // Either Admin or Tutor can access
    public async Task<ActionResult<IEnumerable<Guid>>> GetAssignedCoursesAsync([FromRoute] Guid tutorId)
    {
        try
        {
            var courses = await _tutorService.GetAssignedCoursesAsync(tutorId);
            return Ok(courses);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}