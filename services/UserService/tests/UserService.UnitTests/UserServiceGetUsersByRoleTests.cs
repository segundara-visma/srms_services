using Moq;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Domain.Entities;

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

            var result = await _userService.GetUsersByRoleAsync("Invalid");

            result.Should().BeEmpty();
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
    }
}