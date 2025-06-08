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

public class TutorMeTests : BaseTest
{
    private readonly ITutorService _tutorService;

    public TutorMeTests()
    {
        _tutorService = new TutorServiceImpl(TutorRepositoryMock.Object, UserServiceClientMock.Object, GradeServiceClientMock.Object);
    }

    [Fact]
    public async Task GetMe_ValidUserId_ReturnsTutorDTO()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tutor = CreateTestTutor(userId: userId);
        var profile = new Profile { Address = "789 Pine St", Phone = "555-7890" };
        var userDTO = CreateTestUserDTO(userId, profile);

        MockGetTutorByUserIdAsync(userId, tutor);
        MockGetUserByIdAsync(userId, userDTO);

        // Act
        var result = await _tutorService.GetTutorByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(tutor.Id);
        result.UserId.Should().Be(userId);
        result.FirstName.Should().Be(userDTO.FirstName); // "Tutor"
        result.LastName.Should().Be(userDTO.LastName);   // "One"
        result.Email.Should().Be(userDTO.Email);        // "tutor1@example.com"
        result.Role.Should().Be(userDTO.Role);          // "Tutor"
        result.Profile.Should().NotBeNull();
        result.Profile.Address.Should().Be(profile.Address);
        result.Profile.Phone.Should().Be(profile.Phone);
    }

    [Fact]
    public async Task GetMe_NonExistingTutor_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        MockGetTutorByUserIdAsync(userId, null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _tutorService.GetTutorByIdAsync(userId));
        Assert.Contains($"Tutor with ID {userId} not found", exception.Message);
    }

    [Fact]
    public async Task GetMe_NonTutorUser_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tutor = CreateTestTutor(userId: userId);
        var userDTO = new UserDTO(userId, "NonTutor", "User", "nontutor@example.com", "Student", null);

        MockGetTutorByUserIdAsync(userId, tutor);
        MockGetUserByIdAsync(userId, userDTO);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _tutorService.GetTutorByIdAsync(userId));
        Assert.Contains($"User with ID {userId} is not a tutor", exception.Message);
    }

    [Fact]
    public async Task UpdateMe_ValidRequest_ReturnsTutorDTO()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tutor = CreateTestTutor(userId: userId);
        var profile = new Profile { Address = "101 Maple St", Phone = "555-1010" };
        var updateRequest = CreateTestUpdateRequest(userId, profile.Address, profile.Phone);
        var updatedUserDTO = new UserDTO(userId, updateRequest.FirstName, updateRequest.LastName, updateRequest.Email, "Tutor", profile);

        MockGetTutorByUserIdAsync(userId, tutor);
        MockUpdateUserAsync(userId, updateRequest, updatedUserDTO);

        // Act
        var result = await _tutorService.UpdateTutorAsync(userId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(tutor.Id);
        result.UserId.Should().Be(userId);
        result.FirstName.Should().Be(updateRequest.FirstName); // "Tutor"
        result.LastName.Should().Be(updateRequest.LastName);   // "Two"
        result.Email.Should().Be(updateRequest.Email);        // "tutor2@example.com"
        result.Role.Should().Be("Tutor"); // Assuming role remains unchanged
        result.Profile.Should().NotBeNull();
        result.Profile.Address.Should().Be(profile.Address);
        result.Profile.Phone.Should().Be(profile.Phone);
    }

    [Fact]
    public async Task UpdateMe_NonExistingTutor_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateRequest = CreateTestUpdateRequest(userId);

        MockGetTutorByUserIdAsync(userId, null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _tutorService.UpdateTutorAsync(userId, updateRequest));
        Assert.Contains($"Tutor with ID {userId} not found", exception.Message);
    }

    [Fact]
    public async Task UpdateMe_FailedUpdate_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tutor = CreateTestTutor(userId: userId);
        var updateRequest = CreateTestUpdateRequest(userId);

        MockGetTutorByUserIdAsync(userId, tutor);
        MockUpdateUserAsync(userId, updateRequest, null); // Simulate failed update

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _tutorService.UpdateTutorAsync(userId, updateRequest));
        Assert.Contains("Update request failed", exception.Message);
    }
}