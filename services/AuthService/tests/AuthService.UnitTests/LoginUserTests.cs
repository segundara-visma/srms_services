using System;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using FluentAssertions;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AuthService.UnitTests
{
    public class LoginUserTests : BaseTestClass
    {
        public LoginUserTests() : base() { }

        [Fact]
        public async Task LoginUser_Should_Return_Token_When_Valid_Credentials()
        {
            var validEmail = "test@example.com";
            var validPassword = "password123";

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = validEmail,
                Firstname = "First",
                Lastname = "Last",
                Role = "Student"
            };

            _userServiceMock.Setup(repo => repo.GetUserByEmailAsync(validEmail)).ReturnsAsync(user);
            _userServiceMock.Setup(repo => repo.ValidatePasswordAsync(user.Id, validPassword)).ReturnsAsync(true);
            _jwtTokenHelperMock.Setup(helper => helper.GenerateAccessToken(user.Id, user.Email, user.Role)) // Include userRole
                .Returns("fake-access-token");
            _jwtTokenHelperMock.Setup(helper => helper.GenerateRefreshToken(user.Id, user.Email))
                .Returns("fake-refresh-token");

            var responseMock = SetupHttpContextForCookies();

            var result = await _authService.LoginUser(validEmail, validPassword);

            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();

            _jwtTokenHelperMock.Verify(helper => helper.GenerateAccessToken(user.Id, user.Email, user.Role), Times.Once); // Verify with userRole
            responseMock.Verify(r => r.Cookies.Append("RefreshToken", It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Once);
        }

        [Fact]
        public async Task LoginUser_Should_Throw_Exception_When_User_Not_Found()
        {
            var invalidEmail = "nonexistent@example.com";
            var password = "somepassword";
            _userServiceMock.Setup(repo => repo.GetUserByEmailAsync(invalidEmail)).ReturnsAsync((User?)null);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.LoginUser(invalidEmail, password));
            Assert.Equal("Invalid email or password.", exception.Message);
        }

        [Fact]
        public async Task LoginUser_Should_Throw_Exception_When_Password_Is_Incorrect()
        {
            var validEmail = "test@example.com";
            var invalidPassword = "wrongpassword";

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = validEmail,
                Firstname = "First",
                Lastname = "Last",
                Role = "Student"
            };

            _userServiceMock.Setup(repo => repo.GetUserByEmailAsync(validEmail)).ReturnsAsync(user);
            _userServiceMock.Setup(repo => repo.ValidatePasswordAsync(user.Id, invalidPassword)).ReturnsAsync(false);

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginUser(validEmail, invalidPassword));
            Assert.Equal("Invalid email or password.", exception.Message);
        }

        [Fact]
        public async Task LoginUser_Should_Throw_Exception_When_Email_Or_Password_Is_Empty()
        {
            var emptyEmail = "";
            var emptyPassword = "";

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _authService.LoginUser(emptyEmail, emptyPassword));
            Assert.Equal("Email and password cannot be empty.", exception.Message);
        }
    }
}