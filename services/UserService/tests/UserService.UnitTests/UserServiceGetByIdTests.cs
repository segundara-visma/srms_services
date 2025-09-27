using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using UserService.Domain.Entities;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using Moq;

namespace UserService.UnitTests
{
    public class UserServiceGetByIdTests : BaseTest
    {
        [Fact]
        public async Task GetByIdAsync_Should_Return_User_When_User_Exists()
        {
            // Arrange
            var user = CreateTestUser();
            MockGetByIdAsync(user);

            // Act
            var result = await _userService.GetByIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Email.Should().Be(user.Email);
            result.Firstname.Should().Be(user.Firstname);
            result.Lastname.Should().Be(user.Lastname);
            result.Role.Should().Be("Student"); // Assuming Role is populated via repository
            result.Profile.Should().NotBeNull();
            result.Profile.Address.Should().Be(user.Profile.Address);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_User_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            result.Should().BeNull();
        }
    }
}