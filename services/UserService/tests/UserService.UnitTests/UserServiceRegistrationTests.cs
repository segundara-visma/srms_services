using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Moq;                      // For Moq's mocking capabilities (Times, It, etc.)
using UserService.Domain.Entities;  // For User and Role definitions
using UserService.Application.Interfaces;  // For IUserService and IUserRepository
using System.Threading.Tasks;  // For async methods

namespace UserService.UnitTests
{
    public class UserServiceRegistrationTests : BaseTest
    {
        [Fact]
        public async Task RegisterUser_Should_Create_New_User_When_Valid()
        {
            // Arrange
            var user = CreateTestUser();
            var plainPassword = "password123";
            var roleName = "Student";
            var role = CreateTestRole(roleName);

            // Mock methods for repository
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email)).ReturnsAsync((User?)null); // No user exists with this email
            MockAddUserAsync();  // Make sure AddAsync will succeed
            _userRepositoryMock.Setup(repo => repo.GetRoleByNameAsync(roleName)).ReturnsAsync(role);  // Mock role retrieval

            // Act
            await _userService.RegisterUser(user, plainPassword, roleName);

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);  // Ensure AddAsync is called once
        }

        [Fact]
        public async Task RegisterUser_Should_Throw_Exception_When_Email_Already_Exists()
        {
            // Arrange
            var user = CreateTestUser();
            // Mock that a user already exists with the same email
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterUser(user, "password123", "Student"));
            exception.Message.Should().Be("Email already exists.");
        }

        [Fact]
        public async Task RegisterUser_Should_Throw_Exception_When_Role_Is_Invalid()
        {
            // Arrange
            var user = CreateTestUser();
            var roleName = "InvalidRole";
            _userRepositoryMock.Setup(repo => repo.GetRoleByNameAsync(roleName)).ReturnsAsync((Role?)null);  // Invalid role

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterUser(user, "password123", roleName));
            exception.Message.Should().Be("Invalid role.");
        }
    }
}
