using System;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AuthService.UnitTests
{
    public class LogoutTests : BaseTestClass
    {
        public LogoutTests() : base() { }

        [Fact]
        public async Task Logout_Should_Deactivate_RefreshToken_In_Database()
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
            var refreshTokenFromDb = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsActive = true
            };

            _refreshTokenRepositoryMock.Setup(repo => repo.GetByTokenAsync(refreshToken)).ReturnsAsync(refreshTokenFromDb);
            var responseMock = SetupHttpContextForCookies();

            await _authService.Logout(refreshToken);

            _refreshTokenRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<RefreshToken>(r => !r.IsActive)), Times.Once);
            responseMock.Verify(r => r.Cookies.Delete("RefreshToken"), Times.Once);
        }
    }
}
