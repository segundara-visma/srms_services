using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TutorService.Application.Interfaces;
using TutorService.Application.DTOs;

namespace TutorService.API.Controllers;

/// <summary>
/// Controller for handling service-to-service API operations, restricted to Auth0-authenticated requests.
/// </summary>
[ApiController]
[Route("api/s2s/tutors")]
[Authorize(AuthenticationSchemes = "Auth0")]
public class ServiceToServiceTutorsController : ControllerBase
{
    private readonly ITutorService _tutorService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TutorsController"/> class.
    /// </summary>
    /// <param name="tutorService">The tutor service responsible for business logic.</param>
    public ServiceToServiceTutorsController(ITutorService tutorService)
    {
        _tutorService = tutorService ?? throw new ArgumentNullException(nameof(tutorService));
    }

    /// <summary>
    /// Creates a tutor record for the specified user, called by AdminService.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to create a tutor record for.</param>
    /// <returns>
    /// A 204 No Content response if the tutor record is successfully created.
    /// </returns>
    /// <response code="204">Tutor record was successfully created.</response>
    /// <response code="400">If the input is invalid or the user does not have the Tutor role.</response>
    /// <response code="409">If a tutor record already exists for the user.</response>
    [HttpPost("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTutorAsync(Guid userId)
    {
        try
        {
            await _tutorService.CreateTutorAsync(userId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a list of all tutors in the system.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{TutorDTO}"/> containing details of all tutors.
    /// </returns>
    /// <response code="200">Returns a list of all tutors.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TutorDTO>>> GetAllTutorsAsync()
    {
        var tutors = await _tutorService.GetAllTutorsAsync();
        return Ok(tutors);
    }

    /// <summary>
    /// Assigns a course to a specific tutor.
    /// </summary>
    /// <param name="tutorId">The unique identifier of the tutor to assign the course to.</param>
    /// <param name="courseId">The unique identifier of the course to assign.</param>
    /// <returns>
    /// A 204 No Content response if the assignment is successful, a 400 Bad Request if the tutor is already assigned to the course, or a 404 Not Found if the tutor does not exist.
    /// </returns>
    /// <response code="204">If the course was successfully assigned to the tutor.</response>
    /// <response code="400">If the tutor is already assigned to the course.</response>
    /// <response code="404">If the tutor with the specified ID is not found.</response>
    [HttpPost("{tutorId}/courses")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignCourseToTutorAsync(
        [FromRoute] Guid tutorId,
        [FromQuery] Guid courseId)
    {
        try
        {
            await _tutorService.AssignCourseToTutorAsync(tutorId, courseId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}