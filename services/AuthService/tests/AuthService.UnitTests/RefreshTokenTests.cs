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
            _jwtTokenHelperMock.Setup(helper => helper.GenerateAccessToken(userId, user.Email, user.Role)) // Include userRole
                .Returns("new-access-token");
            _jwtTokenHelperMock.Setup(helper => helper.GenerateRefreshToken(userId, user.Email))
                .Returns("new-refresh-token");

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
            var expiredRefreshToken = "expired-refresh-token";
            _jwtTokenHelperMock.Setup(helper => helper.ValidateRefreshToken(expiredRefreshToken)).Throws<SecurityTokenExpiredException>();

            await Assert.ThrowsAsync<SecurityTokenExpiredException>(() => _authService.RefreshToken(expiredRefreshToken));
        }

        [Fact]
        public async Task RefreshToken_Should_Throw_Exception_When_User_Not_Found()
        {
            var refreshToken = "valid-refresh-token";
            var invalidUserEmail = "nonexistent@example.com";

            var principalMock = new Mock<ClaimsPrincipal>();
            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.Setup(i => i.Name).Returns(Guid.NewGuid().ToString());
            principalMock.Setup(p => p.Identity).Returns(identityMock.Object);
            principalMock.Setup(p => p.FindFirst("email")).Returns(new Claim("email", invalidUserEmail));

            _jwtTokenHelperMock.Setup(helper => helper.ValidateRefreshToken(refreshToken)).Returns(principalMock.Object);
            _userServiceMock.Setup(repo => repo.GetUserByEmailAsync(invalidUserEmail)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RefreshToken(refreshToken));
        }
    }
}