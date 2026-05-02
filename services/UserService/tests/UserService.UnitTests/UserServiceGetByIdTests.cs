using Moq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using System;

namespace UserService.UnitTests
{
    public class UserServiceGetByIdTests : BaseTest
    {
        [Fact]
        public async Task Should_Return_UserResponseDTO_When_User_Exists()
        {
            var user = CreateTestUser();
            MockGetByIdAsync(user);

            var result = await _userService.GetByIdAsync(user.Id);

            result.Should().NotBeNull();
            result!.Email.Should().Be(user.Email);
            result.Role.Should().Be("Student");
            result.Profile.Should().NotBeNull();
            result.Profile!.Address.Should().Be(user.Profile!.Address);
        }

        [Fact]
        public async Task Should_Return_Null_When_User_Not_Found()
        {
            MockGetByIdAsync(null);

            var result = await _userService.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }
    }
}