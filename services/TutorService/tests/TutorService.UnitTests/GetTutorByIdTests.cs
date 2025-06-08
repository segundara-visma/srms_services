using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using TutorService.Domain.Entities;
using TutorService.Application.Services;
using TutorService.Application.Interfaces;
using TutorService.Application.DTOs;
using Xunit;

namespace TutorService.UnitTests;

public class GetTutorByIdTests : BaseTest
{
    private readonly ITutorService _tutorService;

    public GetTutorByIdTests()
    {
        _tutorService = new TutorServiceImpl(TutorRepositoryMock.Object, UserServiceClientMock.Object, GradeServiceClientMock.Object);
    }

    [Fact]
    public async Task GetTutorByIdAsync_WhenTutorExists_ReturnsTutorDTO()
    {
        // Arrange
        var tutorId = Guid.NewGuid();
        var tutor = CreateTestTutor(userId: tutorId);
        var profile = new Profile { Address = "789 Pine St" };
        var user = CreateTestUserDTO(tutorId, profile);
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(tutorId)).ReturnsAsync(tutor);
        UserServiceClientMock.Setup(c => c.GetUserByIdAsync(tutorId)).ReturnsAsync(user);

        // Act
        var result = await _tutorService.GetTutorByIdAsync(tutorId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(tutor.Id);
        result.UserId.Should().Be(tutorId);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.Email.Should().Be(user.Email);
        result.Role.Should().Be(user.Role);
        result.Profile.Should().NotBeNull();
        result.Profile.Address.Should().Be(profile.Address);
    }

    [Fact]
    public async Task GetTutorByIdAsync_WhenTutorNotFound_ThrowsException()
    {
        // Arrange
        var tutorId = Guid.NewGuid();
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(tutorId)).ReturnsAsync((Tutor)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _tutorService.GetTutorByIdAsync(tutorId));
    }

    [Fact]
    public async Task GetTutorByIdAsync_WhenNonTutorUser_ThrowsException()
    {
        // Arrange
        var tutorId = Guid.NewGuid();
        var tutor = CreateTestTutor(userId: tutorId);
        var user = new UserDTO(tutorId, "NonTutor", "User", "nontutor@example.com", "Student", null);
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(tutorId)).ReturnsAsync(tutor);
        UserServiceClientMock.Setup(c => c.GetUserByIdAsync(tutorId)).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _tutorService.GetTutorByIdAsync(tutorId));
    }
}