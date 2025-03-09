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
[Route("api/authservice")]
[ApiController]
[Authorize(AuthenticationSchemes = "Auth0")]  // Only accessible to AuthService via Auth0 token
public class AuthServiceController : ControllerBase
{
    private readonly IUserService _userService;

    // Constructor injection
    public AuthServiceController(IUserService userService)
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
                Role = user.Role.ToString()
            };

            Console.WriteLine($"Successfully retrieved user: {response.Email}");
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
}
