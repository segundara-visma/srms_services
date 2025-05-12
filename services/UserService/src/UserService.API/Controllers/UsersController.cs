using Microsoft.AspNetCore.Mvc;
using UserService.Application.Interfaces;  // For IUserService
using UserService.Domain.Entities;
using UserService.Application.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace UserService.Api.Controllers;

/// <summary>
/// Controller for handling user-related API operations.
/// </summary>
[Route("api/[controller]")]
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

        var user = await _userService.GetByIdAsync(id);  // Use service to get user
        if (user == null)
        {
            return NotFound();
        }

        var response = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Role = user.Role.ToString(),
            Profile = user.Profile
        };

        return Ok(response);
    }

    /// <summary>
    /// Register new user.
    /// </summary>
    /// <param name="request">Object containing the user's email, password, firstname, lastname and role.</param>
    /// <returns>A register-response object.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterRequest request)
    {
        // Hash the password before saving
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Email = request.Email,
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            PasswordHash = hashedPassword
        };

        await _userService.RegisterUser(user, request.Password, request.Role);

        var response = new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email
        };

        return Ok(response);
    }

    /// <summary>
    /// Update existing user.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <param name="request">Object containing the user's details: email, firstname, lastname and profile data.</param>
    /// <returns>An update-user-response object.</returns>
    [HttpPut("{id}")]
    [Authorize]  // Validate the credentials before going ahead to process the request
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest("User ID mismatch");
        }

        var response = await _userService.UpdateAsync(id, request);

        return Ok(response);
    }
}
