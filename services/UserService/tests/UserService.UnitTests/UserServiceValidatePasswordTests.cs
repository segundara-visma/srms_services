using Moq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using System;

namespace UserService.UnitTests
{
    public class UserServiceValidatePasswordTests : BaseTest
    {
        [Fact]
        public async Task Should_Return_True_When_Password_Is_Correct()
        {
            var user = CreateTestUser();
            MockGetByIdAsync(user);

            var result = await _userService.ValidatePasswordAsync(user.Id, "password123");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Return_False_When_Password_Is_Wrong()
        {
            var user = CreateTestUser();
            MockGetByIdAsync(user);

            var result = await _userService.ValidatePasswordAsync(user.Id, "wrong");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task Should_Return_False_When_User_Not_Found()
        {
            MockGetByIdAsync(null);

            var result = await _userService.ValidatePasswordAsync(Guid.NewGuid(), "password");

            result.Should().BeFalse();
        }
    }
}