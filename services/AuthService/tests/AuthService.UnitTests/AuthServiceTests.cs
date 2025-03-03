using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AuthService.Application.Helpers;
using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using AuthService.Domain.Entities;

namespace AuthService.UnitTests;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _authRepositoryMock;
    private readonly Mock<JWTTokenHelper> _jwtTokenHelperMock;
    private readonly IAuthService _authService;

    // Constructor to initialize the mocks
    public AuthServiceTests()
    {
        _authRepositoryMock = new Mock<IAuthRepository>();
        _jwtTokenHelperMock = new Mock<JWTTokenHelper>();

        _authService = new AuthServices(_authRepositoryMock.Object, _jwtTokenHelperMock.Object);
    }

    // Test case for valid login
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        string email = "testuser";
        string password = "testpassword";
        string expectedToken = "jwt-token";

        _authRepositoryMock.Setup(repo => repo.ValidateUserCredentialsAsync(email, password))
            .ReturnsAsync(true);

        // Setup the token service to generate a token
        _jwtTokenHelperMock.Setup(service => service.GenerateToken(It.IsAny<string>()))
            .Returns(expectedToken);

        // Act
        var result = await _authService.LoginUser(email, password);

        // Assert
        Assert.True(expectedToken, result);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ReturnsFalse()
    {
        // Arrange
        string email = "testuser";
        string password = "wrongpassword";

        // Setup the repository to return invalid credentials
        _authRepositoryMock.Setup(repo => repo.ValidateUserCredentialsAsync(email, password))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() => _authService.LoginUser(email, password));
    }

    [Fact]
    public async Task LogoutAsync_ValidSession_ReturnsTrue()
    {
        // Arrange
        string validToken = "jwt-token";

        // Setup the session to check that the token is valid
        _authRepositoryMock.Setup(session => session.IsTokenValid(validToken))
            .Returns(true);

        // Act
        var result = await _authService.LogoutUser(validToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task LogoutAsync_NoActiveSession_ReturnsFalse()
    {
        // Arrange
        string invalidToken = "invalid-jwt-token";

        // Setup the session to check that the token is invalid
        _authRepositoryMock.Setup(session => session.IsTokenValid(invalidToken))
            .Returns(false);

        // Act
        var result = await _authService.LogoutUser(invalidToken);

        // Assert
        Assert.False(result);
    }
}
