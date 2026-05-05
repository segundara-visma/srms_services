using Moq;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Domain.Entities;
using UserService.Application.Exceptions;

namespace UserService.UnitTests
{
    public class UserServiceGetUsersByRoleTests : BaseTest
    {
        [Fact]
        public async Task Should_Return_Users_When_Role_Exists()
        {
            var role = CreateTestRole();

            var users = new List<User>
            {
                CreateTestUser(),
                CreateTestUser()
            };

            MockGetRoleByNameAsync(role);
            MockGetUsersByRoleIdAsync(role.Id, users);

            var result = await _userService.GetUsersByRoleAsync(role.Name);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Should_Return_Empty_When_Role_Not_Found()
        {
            MockGetRoleByNameAsync(null);

            var act = async () => await _userService.GetUsersByRoleAsync("Invalid");

            var exception = await Assert.ThrowsAsync<ApiException>(act);

            exception.StatusCode.Should().Be(404);
            exception.Message.Should().Be("Role not found.");
        }

        [Fact]
        public async Task Should_Return_Paginated_Result()
        {
            var role = CreateTestRole();
            var users = new List<User>
            {
                CreateTestUser(), CreateTestUser(),
                CreateTestUser(), CreateTestUser()
            };

            MockGetRoleByNameAsync(role);
            MockGetUsersByRoleIdPaginated(role.Id, users, users.Count, 2, 2);

            var result = await _userService.GetUsersByRoleAsync(role.Name, 2, 2);

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(4);
            result.Page.Should().Be(2);
        }

        [Fact]
        public async Task Should_Throw_When_Role_Is_Empty()
        {
            var act = async () => await _userService.GetUsersByRoleAsync("");

            var exception = await Assert.ThrowsAsync<ApiException>(act);

            exception.StatusCode.Should().Be(400);
            exception.Message.Should().Be("Role cannot be empty.");
        }

        [Fact]
        public async Task Should_Throw_When_Role_Not_Found_Paginated()
        {
            MockGetRoleByNameAsync(null);

            var act = async () => await _userService.GetUsersByRoleAsync("Invalid", 1, 10);

            var exception = await Assert.ThrowsAsync<ApiException>(act);

            exception.StatusCode.Should().Be(404);
        }
    }
}