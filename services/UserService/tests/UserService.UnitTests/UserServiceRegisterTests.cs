using Moq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using UserService.Domain.Entities;
using UserService.Application.Exceptions;

namespace UserService.UnitTests
{
    public class UserServiceRegisterTests : BaseTest
    {
        [Fact]
        public async Task Should_Register_User()
        {
            var user = CreateTestUser();
            var role = CreateTestRole();

            MockGetByEmailAsync(null);
            MockGetRoleByNameAsync(role);
            MockAddUserAsync();

            await _userService.RegisterUser(user, "password123", role.Name);

            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_When_Email_Exists()
        {
            var user = CreateTestUser();

            MockGetByEmailAsync(user);

            var act = async () =>
                await _userService.RegisterUser(user, "password", "Student");

            var exception = await Assert.ThrowsAsync<ApiException>(act);

            exception.StatusCode.Should().Be(400);
            exception.Message.Should().Be("Email already exists.");
        }

        [Fact]
        public async Task Should_Throw_When_Role_Is_Invalid()
        {
            var user = CreateTestUser();

            MockGetByEmailAsync(null);
            MockGetRoleByNameAsync(null);

            var act = async () =>
                await _userService.RegisterUser(user, "password", "Invalid");

            var exception = await Assert.ThrowsAsync<ApiException>(act);

            exception.StatusCode.Should().Be(400);
            exception.Message.Should().Be("Invalid role.");
        }
    }
}