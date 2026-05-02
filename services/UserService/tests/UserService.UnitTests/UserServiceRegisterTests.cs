using Moq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using UserService.Domain.Entities;

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

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userService.RegisterUser(user, "password", "Student"));
        }
    }
}