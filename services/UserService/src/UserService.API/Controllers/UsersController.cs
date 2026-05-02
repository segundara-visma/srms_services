using Microsoft.AspNetCore.Mvc;
using UserService.Application.Interfaces;  // For IUserService
using UserService.Domain.Entities;
using UserService.Application.Common;
using UserService.Application.DTOs;
using UserService.Application.Mappers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace UserService.Api.Controllers;

/// <summary>
/// Controller for handling user-related API operations.
/// </summary>
[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;  // Use IUserService (which can be UserService)

    // Constructor injection for IUserService
    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController"/> class.
    /// </summary>
    /// <param name="userService">The user service.</param>
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <returns>A user object.</returns>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Update existing user.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <param name="request">Object containing the user's details: email, firstname, lastname and profile data.</param>
    /// <returns>An update-user-response object.</returns>
    [HttpPut("{id}")]
    [Authorize]  // Validate the credentials before going ahead to process the request
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateRequestDTO request)
    {
        if (id != request.Id)
            return BadRequest("User ID mismatch");

        var response = await _userService.UpdateAsync(id, request);

        return Ok(response);
    }

    /// <summary>
    /// Retrieves users in batch by IDs.
    /// </summary>
    [HttpGet("batch")]
    public async Task<IActionResult> GetUsersByIds([FromQuery] List<Guid> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest("Ids are required");

        var users = await _userService.GetByIdsAsync(ids);

        return Ok(users);
    }
}
