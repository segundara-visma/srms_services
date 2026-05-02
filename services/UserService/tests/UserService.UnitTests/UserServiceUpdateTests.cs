using Moq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using UserService.Application.DTOs;

namespace UserService.UnitTests
{
    public class UserServiceUpdateTests : BaseTest
    {
        [Fact]
        public async Task Should_Update_User()
        {
            var user = CreateTestUser();

            var request = new UpdateRequestDTO(
                user.Id,
                null,            // Email
                "Updated",       // Firstname
                null,            // Lastname
                "New Address",   // Address
                null, null, null, null, null,
                null, null, null, null, null, null, null
            );

            MockGetByIdAsync(user);
            MockUpdateUserAsync();

            var result = await _userService.UpdateAsync(user.Id, request);

            result.Should().NotBeNull();
            result!.Firstname.Should().Be("Updated");
            result.Profile!.Address.Should().Be("New Address");
        }

        [Fact]
        public async Task Should_Throw_When_User_Not_Found()
        {
            MockGetByIdAsync(null);

            await Assert.ThrowsAsync<Exception>(() =>
                _userService.UpdateAsync(Guid.NewGuid(),
                    new UpdateRequestDTO(
                        Guid.NewGuid(),
                        null, null, null,
                        null, null, null, null, null, null,
                        null, null, null, null, null, null, null
                    )
                )
            );
        }
    }
}