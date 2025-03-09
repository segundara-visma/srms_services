using Microsoft.AspNetCore.Mvc;
using AuthService.Application.Interfaces;  // For IAuthService (or IUserService)
using AuthService.Domain.Entities;
using AuthService.Application.DTOs;
using System.Threading.Tasks;
using AuthService.Application.Helpers;  // Include the Helpers namespace

namespace AuthService.Api.Controllers
{
    /// <summary>
    /// Controller for handling user-related API operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly JWTTokenHelper _jwtTokenHelper;

        // Constructor injection for IAuthService
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        /// <param name="jwtTokenHelper">The JWT token helper.</param>
        public AuthController(IAuthService authService, JWTTokenHelper jwtTokenHelper)
        {
            _authService = authService;
            _jwtTokenHelper = jwtTokenHelper;
        }

        /// <summary>
        /// Authenticate user.
        /// </summary>
        /// <param name="request">Object containing the user's email and password.</param>
        /// <returns>A login-response object.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Attempt to login the user and get the login response from UserService
                var response = await _authService.LoginUser(request.Email, request.Password);

                if (response == null)
                {
                    // Return Unauthorized if response is null (shouldn't happen if exceptions are handled properly)
                    return Unauthorized("Invalid email or password.");
                }

                // Return the login response with the access token
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                // This exception occurs when the user is not found
                //return Unauthorized("Invalid email or password.");
                return Unauthorized(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                // This exception occurs when the password is incorrect
                //return Unauthorized("Invalid email or password.");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                // Catch any other unexpected exceptions and return a 500 Internal Server Error
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Generate a new access token using the refresh token.
        /// </summary>
        /// <returns>A new access token.</returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            // Retrieve the refresh token from the cookie
            var refreshToken = Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("No refresh token found.");
            }

            try
            {
                // Call the UserService to handle the refresh token logic
                var response = await _authService.RefreshToken(refreshToken);

                // Return the new access token and refresh token
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Log out the user.
        /// </summary>
        /// <returns>A response indicating the status of the logout operation.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Retrieve the refresh token from the cookie
            var refreshToken = Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("No refresh token found.");
            }

            //var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var token = Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("No token provided.");
            }

            // Revoke the token
            await _authService.RevokeTokenAsync(token);

            // Attempt to log out the user by invalidating the refresh token
            await _authService.Logout(refreshToken);  // Call Logout without expecting any return value

            return Ok(new { message = "Logged out successfully" });
        }
    }
}
