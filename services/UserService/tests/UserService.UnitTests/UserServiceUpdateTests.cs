using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using UserService.Domain.Entities;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using Moq;

namespace UserService.UnitTests
{
    public class UserServiceUpdateTests : BaseTest
    {
        [Fact]
        public async Task UpdateAsync_Should_Update_User_When_User_Exists()
        {
            // Arrange
            var user = CreateTestUser();
            var request = new UpdateRequest
            {
                Id = user.Id,
                Firstname = "UpdatedFirst",
                Lastname = "UpdatedLast",
                Email = "updated@example.com",
                Address = "456 Updated St.",
                Phone = "555-1234",
                City = "City",
                State = "State",
                ZipCode = "12345",
                Country = "Country",
                Nationality = "Nationality",
                Bio = "Bio",
                FacebookUrl = "fb.com/test",
                TwitterUrl = "twitter.com/test",
                LinkedInUrl = "linkedin.com/test",
                InstagramUrl = "insta.com/test",
                WebsiteUrl = "website.com"
            };
            MockGetByIdAsync(user);
            MockUpdateUserAsync(user);

            // Act
            var result = await _userService.UpdateAsync(user.Id, request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Firstname.Should().Be(request.Firstname);
            result.Lastname.Should().Be(request.Lastname);
            result.Email.Should().Be(request.Email);
            result.Profile.Should().NotBeNull();
            result.Profile.Address.Should().Be(request.Address);
            result.Profile.Phone.Should().Be(request.Phone);
            result.Profile.City.Should().Be(request.City);
            result.Profile.State.Should().Be(request.State);
            result.Profile.ZipCode.Should().Be(request.ZipCode);
            result.Profile.Country.Should().Be(request.Country);
            result.Profile.Nationality.Should().Be(request.Nationality);
            result.Profile.Bio.Should().Be(request.Bio);
            result.Profile.FacebookUrl.Should().Be(request.FacebookUrl);
            result.Profile.TwitterUrl.Should().Be(request.TwitterUrl);
            result.Profile.LinkedInUrl.Should().Be(request.LinkedInUrl);
            result.Profile.InstagramUrl.Should().Be(request.InstagramUrl);
            result.Profile.WebsiteUrl.Should().Be(request.WebsiteUrl);
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(user), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_Exception_When_User_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateRequest { Id = userId, Firstname = "UpdatedFirst", Lastname = "UpdatedLast", Email = "updated@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userService.UpdateAsync(userId, request));
            exception.Message.Should().Be("User not found");
        }

        [Fact]
        public async Task UpdateAsync_Should_Keep_Existing_Values_When_Request_Values_Are_Null()
        {
            // Arrange
            var user = CreateTestUser();
            var request = new UpdateRequest { Id = user.Id };
            MockGetByIdAsync(user);
            MockUpdateUserAsync(user);

            // Act
            var result = await _userService.UpdateAsync(user.Id, request);

            // Assert
            result.Should().NotBeNull();
            result.Firstname.Should().Be(user.Firstname);
            result.Lastname.Should().Be(user.Lastname);
            result.Email.Should().Be(user.Email);
            result.Profile.Should().NotBeNull();
            result.Profile.Address.Should().Be(user.Profile.Address);
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(user), Times.Once());
        }
    }
}