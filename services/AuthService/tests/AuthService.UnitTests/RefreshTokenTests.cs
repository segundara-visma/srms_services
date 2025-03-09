using System;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using FluentAssertions;
using Xunit;
using System.Security.Claims;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.UnitTests
{
    public class RefreshTokenTests : BaseTestClass
    {
        public RefreshTokenTests() : base() { }

        [Fact]
        public async Task RefreshToken_Should_Return_New_Access_Token_When_Valid_Refresh_Token()
        {
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

            _userServiceMock.Setup(repo => repo.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);
            var principalMock = new Mock<ClaimsPrincipal>();
            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.Setup(i => i.Name).Returns(userId.ToString());
            principalMock.Setup(p => p.Identity).Returns(identityMock.Object);
            principalMock.Setup(p => p.FindFirst("email")).Returns(new Claim("email", user.Email));

            _jwtTokenHelperMock.Setup(helper => helper.ValidateRefreshToken(refreshToken)).Returns(principalMock.Object);
            _jwtTokenHelperMock.Setup(helper => helper.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>())).Returns("new-access-token");
            _jwtTokenHelperMock.Setup(helper => helper.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>())).Returns("new-refresh-token");

            var responseMock = SetupHttpContextForCookies();

            var result = await _authService.RefreshToken(refreshToken);

            result.Should().NotBeNull();
            result.Token.Should().Be("new-access-token");
            responseMock.Verify(r => r.Cookies.Append("RefreshToken", "new-refresh-token", It.IsAny<CookieOptions>()), Times.Once);
        }

        [Fact]
        public async Task RefreshToken_Should_Throw_Exception_When_Invalid_Refresh_Token()
        {
            var refreshToken = "invalid-refresh-token";
            _jwtTokenHelperMock.Setup(helper => helper.ValidateRefreshToken(refreshToken)).Throws<InvalidOperationException>();

            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RefreshToken(refreshToken));
        }

        [Fact]
        public async Task RefreshToken_Should_Throw_Exception_When_Refresh_Token_Is_Expired()
        {
            // Arrange
            var expiredRefreshToken = "expired-refresh-token";
            _jwtTokenHelperMock.Setup(helper => helper.ValidateRefreshToken(expiredRefreshToken)).Throws<SecurityTokenExpiredException>();

            // Act & Assert
            await Assert.ThrowsAsync<SecurityTokenExpiredException>(() => _authService.RefreshToken(expiredRefreshToken));
        }

        [Fact]
        public async Task RefreshToken_Should_Throw_Exception_When_User_Not_Found()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            var invalidUserEmail = "nonexistent@example.com";

            // Mock the refresh token validation to return a valid principal, but the user does not exist
            var principalMock = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var identityMock = new Mock<System.Security.Claims.ClaimsIdentity>();
            identityMock.Setup(i => i.Name).Returns(Guid.NewGuid().ToString());  // Simulate a valid user id
            principalMock.Setup(p => p.Identity).Returns(identityMock.Object);
            principalMock.Setup(p => p.FindFirst("email")).Returns(new Claim("email", invalidUserEmail));

            _jwtTokenHelperMock.Setup(helper => helper.ValidateRefreshToken(refreshToken)).Returns(principalMock.Object);

            // Mock user service to return null for the nonexistent email
            _userServiceMock.Setup(repo => repo.GetUserByEmailAsync(invalidUserEmail)).ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RefreshToken(refreshToken));
        }

    }
}
