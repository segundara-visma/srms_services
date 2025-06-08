using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims; // Added to resolve ClaimTypes

namespace AdminService.API.Controllers;

/// <summary>
/// Controller for handling administrative operations, restricted to Auth0-authenticated admin users.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminsController : ControllerBase
{
    private readonly IAdminService _adminService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminsController"/> class.
    /// </summary>
    /// <param name="adminService">The admin service responsible for business logic.</param>
    public AdminsController(IAdminService adminService)
    {
        _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
    }

    /// <summary>
    /// Retrieves all users with the specified role.
    /// </summary>
    /// <param name="role">The role of the users to retrieve (e.g., "Tutor", "Student").</param>
    /// <returns>
    /// An <see cref="IEnumerable{UserDTO}"/> containing the users with the specified role.
    /// </returns>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="400">If the role is invalid.</response>
    [HttpGet("users/by-role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsersByRoleAsync([FromQuery] string role)
    {
        try
        {
            var users = await _adminService.GetAllUsersByRoleAsync(role);
            return Ok(users);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new user with the specified details.
    /// </summary>
    /// <param name="request">The request containing the user’s details.</param>
    /// <returns>
    /// The unique identifier of the created user.
    /// </returns>
    /// <response code="201">User was successfully created.</response>
    /// <response code="400">If the input is invalid.</response>
    [HttpPost("users")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateUserAsync([FromBody] CreateUserRequest request)
    {
        try
        {
            var userId = await _adminService.CreateUserAsync(request.FirstName, request.LastName, request.Email, request.Password, request.Role);
            //return CreatedAtAction(nameof(CreateUserAsync), new { id = userId }, userId);
            return Created(userId.ToString(), userId); // Returns 201 with the userId in the body
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves the currently signed-in admin.
    /// </summary>
    /// <returns>
    /// An object containing the admin's details if found, or a 401 Not authorized.
    /// </returns>
    /// <response code="200">Returns the admin details.</response>
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
            var admin = await _adminService.GetAdminByIdAsync(Guid.Parse(userId));
            return Ok(admin);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Updates the currently signed-in admin.
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
            await _adminService.UpdateAdminAsync(Guid.Parse(userId), dto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a system-wide overview with counts of tutors, students, and grades.
    /// </summary>
    /// <returns>
    /// A <see cref="SystemOverviewDTO"/> containing the system overview.
    /// </returns>
    /// <response code="200">Returns the system overview.</response>
    [HttpGet("overview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<SystemOverviewDTO>> GetSystemOverviewAsync()
    {
        var overview = await _adminService.GetSystemOverviewAsync();
        return Ok(overview);
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
    /// <response code="400">If the tutor is already assigned to the course or input is invalid.</response>
    /// <response code="404">If the tutor with the specified ID is not found.</response>
    [HttpPost("tutors/{tutorId}/courses")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignCourseToTutorAsync(
        [FromRoute] Guid tutorId,
        [FromQuery] Guid courseId)
    {
        try
        {
            await _adminService.AssignCourseToTutorAsync(tutorId, courseId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

/// <summary>
/// Represents the request payload for creating a new user.
/// </summary>
public record CreateUserRequest(string FirstName, string LastName, string Email, string Password, string Role);
