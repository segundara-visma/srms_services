using Microsoft.AspNetCore.Mvc;
using UserService.Application.Interfaces;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace UserService.Api.Controllers;

/// <summary>
/// Controller for handling service-to-service API operations, restricted to Auth0-authenticated requests.
/// </summary>
[Route("api/s2s/users")]
[ApiController]
[Authorize(AuthenticationSchemes = "Auth0")] // Restrict to Auth0-authenticated service requests
public class ServiceToServiceController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceToServiceController"/> class.
    /// </summary>
    /// <param name="userService">The user service.</param>
    public ServiceToServiceController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get user by email - intended for AuthService.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <returns>A user object.</returns>
    [HttpGet("by-email/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        try
        {
            var user = await _userService.GetByEmailAsync(email);

            if (user == null)
            {
                Console.WriteLine($"User with email {email} not found.");
                return NotFound();
            }

            var response = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Role = user.Role?.Name ?? "Unknown" // Use Role.Name instead of ToString()
                //Role = user.Role.ToString()
            };

            Console.WriteLine($"Successfully retrieved user: {response.Email} with role: {response.Role}");
            return Ok(response);
        }
        catch (Exception ex)
        {
            // Log any exception that occurs during the process
            Console.WriteLine($"Error occurred while retrieving user: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    /// <summary>
    /// Validate user password - intended for AuthService.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>Boolean indicating whether the password is valid.</returns>
    [HttpGet("by-password/{userId}/{password}")]
    public async Task<IActionResult> ValidateUserByPassword(Guid userId, string password)
    {
        var isValid = await _userService.ValidatePasswordAsync(userId, password);

        if (!isValid)
        {
            return Unauthorized();  // Password is incorrect
        }

        return Ok();  // Password is correct
    }

    /// <summary>
    /// Requests to get a user by their unique identifier.
    /// </summary>
    /// <param name="userid">The user's unique identifier.</param>
    /// <returns>A user object.</returns>
    [HttpGet("{userid}")]
    public async Task<IActionResult> StudentServiceRequestUserById(Guid userid)
    {
        var user = await _userService.GetByIdAsync(userid);
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
    /// StudentService requests to get all users with the specified role.
    /// </summary>
    /// <param name="role">The role name (e.g., Student, Tutor, Admin).</param>
    /// <returns>A list of users with the specified role.</returns>
    [HttpGet("by-role")]
    public async Task<IActionResult> GetUsersByRole([FromQuery] string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return BadRequest("Role parameter is required.");
        }

        var users = await _userService.GetUsersByRoleAsync(role);
        return Ok(users);
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

        return Ok(response.UserId);
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
