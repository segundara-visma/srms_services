using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using UserService.Domain.Entities;
using UserService.Application.DTOs;
using Moq;

namespace UserService.UnitTests
{
    public class UserServiceGetUsersByRoleTests : BaseTest
    {
        [Fact]
        public async Task GetUsersByRoleAsync_Should_Return_Paginated_Users_When_Role_Exists()
        {
            // Arrange
            var roleName = "Student";
            var role = CreateTestRole(roleName);
            var users = new List<User> { CreateTestUser(Guid.NewGuid()), CreateTestUser(Guid.NewGuid()) };
            MockGetRoleByNameAsync(role);
            MockGetUsersByRoleIdAsync(role.Id, users, users.Count, page: 1, pageSize: 10);

            // Act
            var result = await _userService.GetUsersByRoleAsync(roleName, page: 1, pageSize: 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(users.Count);
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalCount.Should().Be(users.Count);
            result.TotalPages.Should().Be(1);
            result.Items.Should().ContainEquivalentOf(new UserResponse
            {
                Id = users[0].Id,
                Email = users[0].Email,
                Firstname = users[0].Firstname,
                Lastname = users[0].Lastname,
                Role = roleName,
                Profile = users[0].Profile
            });
        }

        [Fact]
        public async Task GetUsersByRoleAsync_Should_Return_NonPaginated_Users_When_Role_Exists()
        {
            // Arrange
            var roleName = "Student";
            var role = CreateTestRole(roleName);
            var users = new List<User> { CreateTestUser(Guid.NewGuid()), CreateTestUser(Guid.NewGuid()) };
            MockGetRoleByNameAsync(role);
            _userRepositoryMock.Setup(repo => repo.GetUsersByRoleIdAsync(role.Id))
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetUsersByRoleAsync(roleName);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(users.Count);
            result.Should().ContainEquivalentOf(new UserResponse
            {
                Id = users[0].Id,
                Email = users[0].Email,
                Firstname = users[0].Firstname,
                Lastname = users[0].Lastname,
                Role = roleName,
                Profile = users[0].Profile
            });
        }

        [Fact]
        public async Task GetUsersByRoleAsync_Should_Return_Empty_When_Role_Not_Found()
        {
            // Arrange
            var roleName = "InvalidRole";
            _userRepositoryMock.Setup(repo => repo.GetRoleByNameAsync(roleName)).ReturnsAsync((Role?)null);

            // Act
            var result = await _userService.GetUsersByRoleAsync(roleName);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUsersByRoleAsync_Should_Throw_Exception_When_Role_Is_Empty()
        {
            // Arrange
            var roleName = "";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _userService.GetUsersByRoleAsync(roleName, page: 1, pageSize: 10));
            exception.Message.Should().Be("Role cannot be empty. (Parameter 'role')");
        }

        [Fact]
        public async Task GetUsersByRoleAsync_Should_Return_Correct_Page()
        {
            // Arrange
            var roleName = "Student";
            var role = CreateTestRole(roleName);
            var users = new List<User>
            {
                CreateTestUser(Guid.NewGuid()),
                CreateTestUser(Guid.NewGuid()),
                CreateTestUser(Guid.NewGuid()),
                CreateTestUser(Guid.NewGuid())
            };
            MockGetRoleByNameAsync(role);
            MockGetUsersByRoleIdAsync(role.Id, users, users.Count, page: 2, pageSize: 2);

            // Act
            var result = await _userService.GetUsersByRoleAsync(roleName, page: 2, pageSize: 2);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2); // Should get the second page (items 2-3)
            result.Page.Should().Be(2);
            result.PageSize.Should().Be(2);
            result.TotalCount.Should().Be(users.Count);
            result.TotalPages.Should().Be(2);
        }
    }
}