using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using UserService.Domain.Entities;
using Moq;

namespace UserService.UnitTests
{
    public class UserServiceRegisterTests : BaseTest
    {
        [Fact]
        public async Task RegisterUser_Should_Create_New_User_When_Valid()
        {
            // Arrange
            var user = CreateTestUser();
            var plainPassword = "password123";
            var roleName = "Student";
            var role = CreateTestRole(roleName);

            User addedUser = null!; // Non-nullable with null-forgiving operator, as it's set in Callback
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email)).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(repo => repo.GetRoleByNameAsync(roleName)).ReturnsAsync(role);
            _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>())).Callback<User>(u => addedUser = u).Returns(Task.CompletedTask);

            // Act
            await _userService.RegisterUser(user, plainPassword, roleName);

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once());
            addedUser.Should().NotBeNull();
            BCrypt.Net.BCrypt.Verify(plainPassword, addedUser.PasswordHash).Should().BeTrue(); // Verify password matches
            addedUser.RoleId.Should().Be(role.Id);
            addedUser.Id.Should().NotBe(Guid.Empty); // Verify Id is set
        }

        [Fact]
        public async Task RegisterUser_Should_Throw_Exception_When_Email_Already_Exists()
        {
            // Arrange
            var user = CreateTestUser();
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
            _userRepositoryMock.Setup(repo => repo.GetRoleByNameAsync(roleName)).ReturnsAsync((Role?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterUser(user, "password123", roleName));
            exception.Message.Should().Be("Invalid role.");
        }
    }
}