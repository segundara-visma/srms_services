using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using UserService.Domain.Entities;  // For User and Role definitions
using Moq;                      // For Moq's mocking capabilities (Times, It, etc.)

namespace UserService.UnitTests
{
    public class UserServicePasswordValidationTests : BaseTest
    {
        [Fact]
        public async Task ValidatePasswordAsync_Should_Return_True_When_Password_Is_Valid()
        {
            // Arrange
            var user = CreateTestUser();
            var password = "password123";
            // Mock the repository to return the user
            MockGetByIdAsync(user);

            // Act
            var result = await _userService.ValidatePasswordAsync(user.Id, password);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ValidatePasswordAsync_Should_Return_False_When_Password_Is_Invalid()
        {
            // Arrange
            var user = CreateTestUser();
            var password = "invalidpassword";
            // Mock the repository to return the user
            MockGetByIdAsync(user);

            // Act
            var result = await _userService.ValidatePasswordAsync(user.Id, password);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidatePasswordAsync_Should_Return_False_When_User_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var password = "anyPassword";
            // Mock the repository to return null for this user ID
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.ValidatePasswordAsync(userId, password);

            // Assert
            result.Should().BeFalse();
        }
    }
}
