using System;
using System.Threading.Tasks;
using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using AuthService.Domain.Entities;
using Moq;
using Xunit;
using FluentAssertions;
using BCrypt.Net;  // Add this to handle password hashing
using AuthService.Application.Helpers;  // Add this for JWTTokenHelper
using Microsoft.AspNetCore.Http;  // For mocking cookies
using Xunit.Abstractions;
using System.Security.Claims;

namespace AuthService.UnitTests
{
    public class UserServiceTests
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<JWTTokenHelper> _jwtTokenHelperMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly IAuthService _authService;
        private readonly ITestOutputHelper _output;  // Add this line to capture output

        // Constructor to initialize the mocks
        public UserServiceTests(ITestOutputHelper output)
        {
            _output = output;  // Assign to the private field
            _authRepositoryMock = new Mock<IAuthRepository>();
            _userServiceMock = new Mock<IUserService>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _jwtTokenHelperMock = new Mock<JWTTokenHelper>("aVeryLongSecureKeyThatIs256BitsLongUsedForHS256Signing", "AuthServiceIssuer", "AuthServiceAudience");  // Mock constructor parameters
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _authService = new AuthServices(_userServiceMock.Object, _authRepositoryMock.Object, _refreshTokenRepositoryMock.Object, _jwtTokenHelperMock.Object, _httpContextAccessorMock.Object);
        }

        // Helper method to mock the HttpContext and HttpResponse
        private Mock<HttpResponse> SetupHttpContextForCookies()
        {
            var httpContextMock = new Mock<HttpContext>();
            var responseMock = new Mock<HttpResponse>();

            httpContextMock.Setup(ctx => ctx.Response).Returns(responseMock.Object);
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContextMock.Object);

            // Mock Cookies.Append method to verify it was called
            responseMock.Setup(r => r.Cookies.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>())).Verifiable();

            return responseMock;
        }

        // Test case for valid login
        [Fact]
        public async Task LoginUser_Should_Return_Token_When_Valid_Credentials()
        {
            _output.WriteLine("Test Started");

            // Arrange
            var validEmail = "test@example.com";
            var validPassword = "password123";  // Plain password provided by user
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = validEmail,
                Firstname = "First",
                Lastname = "Last",
                Role = "Student"
            };

            // Mock GetUserByEmailAsync to return the user object
            _userServiceMock.Setup(repo => repo.GetUserByEmailAsync(validEmail)).ReturnsAsync(user);

            // Mock ValidatePasswordAsync to return true (valid password)
            _userServiceMock.Setup(repo => repo.ValidatePasswordAsync(user.Id, validPassword)).ReturnsAsync(true);

            // Mock the JWT token generation
            _jwtTokenHelperMock.Setup(helper => helper.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>())).Returns("fake-access-token");
            _jwtTokenHelperMock.Setup(helper => helper.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>())).Returns("fake-refresh-token");

            // Mock HttpContext and HttpResponse setup for capturing cookies
            var responseMock = SetupHttpContextForCookies(); // Reusing the helper method

            // Act
            var result = await _authService.LoginUser(validEmail, validPassword);  // Use plain password for login attempt

            // Debugging: Print the result to ensure the token is set
            _output.WriteLine($"RESULT: {result}");
            _output.WriteLine($"TOKEN: {result?.Token}");  // This will print to the output in the test runner

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty(); // Ensure a token is returned

            // Verify that the mock was called for access token generation
            _jwtTokenHelperMock.Verify(helper => helper.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);

            // Verify that the refresh token was set in the cookies (if needed for the login process)
            responseMock.Verify(r => r.Cookies.Append("RefreshToken", It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Once);
        }

        // Test case for invalid email (user not found)
        [Fact]
        public async Task LoginUser_Should_Throw_Exception_When_User_Not_Found()
        {
            // Arrange
            var invalidEmail = "nonexistent@example.com";
            var password = "somepassword";

            // Mock the repository to return null when looking for the email
            _userServiceMock.Setup(repo => repo.GetUserByEmailAsync(invalidEmail)).ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.LoginUser(invalidEmail, password));
            Assert.Equal("Invalid email or password.", exception.Message);
        }

        // Test case for invalid password
        [Fact]
        public async Task LoginUser_Should_Throw_Exception_When_Password_Is_Incorrect()
        {
            // Arrange
            var validEmail = "test@example.com";
            var invalidPassword = "wrongpassword"; // Incorrect password provided for login

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = validEmail,
                Firstname = "First",
                Lastname = "Last",
                Role = "Student"
            };

            // Mock the repository to return the user
            _userServiceMock.Setup(repo => repo.GetUserByEmailAsync(validEmail)).ReturnsAsync(user);

            // Mock ValidatePasswordAsync to return false for the invalid password
            _userServiceMock.Setup(repo => repo.ValidatePasswordAsync(user.Id, invalidPassword)).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginUser(validEmail, invalidPassword));
            Assert.Equal("Invalid email or password.", exception.Message);
        }

        // Test case for valid refresh token
        [Fact]
        public async Task RefreshToken_Should_Return_New_Access_Token_When_Valid_Refresh_Token()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                Firstname = "First",
                Lastname = "Last",
                Role = "Student"
            };

            // Mock user repository to return the user
            _userServiceMock.Setup(repo => repo.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);

            // Mock refresh token validation to return a valid principal
            var principalMock = new Mock<System.Security.Claims.ClaimsPrincipal>();

            // Mock Identity and Name properties to avoid null dereference
            var identityMock = new Mock<System.Security.Claims.ClaimsIdentity>();
            identityMock.Setup(i => i.Name).Returns(userId.ToString());

            principalMock.Setup(p => p.Identity).Returns(identityMock.Object);
            principalMock.Setup(p => p.FindFirst("email")).Returns(new Claim("email", user.Email));

            _jwtTokenHelperMock.Setup(helper => helper.ValidateRefreshToken(refreshToken)).Returns(principalMock.Object);

            // Mock token generation
            _jwtTokenHelperMock.Setup(helper => helper.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>())).Returns("new-access-token");
            _jwtTokenHelperMock.Setup(helper => helper.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>())).Returns("new-refresh-token");

            // Mock HttpContext and capture cookies setup
            var responseMock = SetupHttpContextForCookies();

            // Act
            var result = await _authService.RefreshToken(refreshToken);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be("new-access-token");

            // Verify the refresh token is being set in the cookie
            responseMock.Verify(r => r.Cookies.Append("RefreshToken", "new-refresh-token", It.IsAny<CookieOptions>()), Times.Once);
        }

        // Test case for invalid refresh token
        [Fact]
        public async Task RefreshToken_Should_Throw_Exception_When_Invalid_Refresh_Token()
        {
            // Arrange
            var refreshToken = "invalid-refresh-token";

            // Mock the refresh token validation to return null (invalid token)
            _jwtTokenHelperMock.Setup(helper => helper.ValidateRefreshToken(refreshToken)).Throws<InvalidOperationException>();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RefreshToken(refreshToken));
        }

        // Test case for logout
        [Fact]
        public async Task Logout_Should_Deactivate_RefreshToken_In_Database()
        {
            // Arrange
            var refreshToken = "valid-refresh-token"; // The token you want to mock in the cookie
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                Firstname = "First",
                Lastname = "Last",
                Role = "Student"
            };

            // Mock the user repository to return the user
            _userServiceMock.Setup(repo => repo.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);

            // Create a mock RefreshTokenRepository to simulate database operations
            var refreshTokenFromDb = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsActive = true
            };

            _refreshTokenRepositoryMock.Setup(repo => repo.GetByTokenAsync(refreshToken))
                                       .ReturnsAsync(refreshTokenFromDb);

            // Use the SetupHttpContextForCookies() method to mock the HttpContext and cookie handling
            var responseMock = SetupHttpContextForCookies(); // This handles cookies and HttpContext setup

            // Act
            await _authService.Logout(refreshToken);

            // Assert
            // Verify that the refresh token's IsActive property was set to false
            _refreshTokenRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<RefreshToken>(r => !r.IsActive)), Times.Once);

            // Verify that the refresh token cookie is deleted using the mocked HttpResponse
            responseMock.Verify(r => r.Cookies.Delete("RefreshToken"), Times.Once);
        }
    }
}
