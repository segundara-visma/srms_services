using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorService.Domain.Entities;
using TutorService.Application.Services;
using TutorService.Application.Interfaces;
using TutorService.Application.DTOs;
using Xunit;

namespace TutorService.UnitTests;

public class GetAllTutorsTests : BaseTest
{
    private readonly ITutorService _tutorService;

    public GetAllTutorsTests()
    {
        _tutorService = new TutorServiceImpl(TutorRepositoryMock.Object, UserServiceClientMock.Object, GradeServiceClientMock.Object);
    }

    [Fact]
    public async Task GetAllTutorsAsync_WhenTutorsExist_ReturnsTutorList()
    {
        // Arrange
        var tutorId1 = Guid.NewGuid();
        var tutorId2 = Guid.NewGuid();
        var users = new List<UserDTO>
        {
            CreateTestUserDTO(tutorId1, new Profile { Address = "789 Pine St" }),
            CreateTestUserDTO(tutorId2)
        };
        var tutors = new List<Tutor>
        {
            CreateTestTutor(userId: tutorId1),
            CreateTestTutor(userId: tutorId2)
        };
        UserServiceClientMock.Setup(c => c.GetUsersByRoleAsync("Tutor")).ReturnsAsync(users);
        foreach (var tutor in tutors)
            MockGetTutorByUserIdAsync(tutor.UserId, tutor);

        // Act
        var result = await _tutorService.GetAllTutorsAsync();

        // Assert
        result.Should().HaveCount(2);
        var tutor1 = result.Single(t => t.UserId == tutorId1);
        tutor1.Email.Should().Be("tutor1@example.com");
        tutor1.Profile.Should().NotBeNull();
        tutor1.Profile.Address.Should().Be("789 Pine St");
        var tutor2 = result.Single(t => t.UserId == tutorId2);
        tutor2.Email.Should().Be("tutor1@example.com");
        tutor2.Profile.Should().BeNull();
    }

    [Fact]
    public async Task GetAllTutorsAsync_WhenNoTutors_ReturnsEmptyList()
    {
        // Arrange
        UserServiceClientMock.Setup(c => c.GetUsersByRoleAsync("Tutor")).ReturnsAsync(new List<UserDTO>());

        // Act
        var result = await _tutorService.GetAllTutorsAsync();

        // Assert
        result.Should().BeEmpty();
    }
}