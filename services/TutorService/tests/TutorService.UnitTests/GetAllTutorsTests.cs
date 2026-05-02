using Moq;
using Moq.Language.Flow;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutorService.Application.DTOs;
using TutorService.Application.Interfaces;
using TutorService.Application.Services;
using TutorService.Domain.Entities;
using Xunit;

namespace TutorService.UnitTests;

public class GetAllTutorsTests : BaseTest
{
    private readonly ITutorService _tutorService;

    public GetAllTutorsTests()
    {
        _tutorService = new TutorServiceImpl(
            TutorRepositoryMock.Object,
            UserServiceClientMock.Object,
            GradeServiceClientMock.Object);
    }

    [Fact]
    public async Task GetAllTutorsAsync_WhenTutorsExist_ReturnsTutorList()
    {
        // Arrange
        var tutorId1 = Guid.NewGuid();
        var tutorId2 = Guid.NewGuid();

        var users = new List<UserDTO>
        {
            CreateTestUserDTO(tutorId1, new ProfileDTO("789 Pine St", null, null, null, null, null, null, null, null, null, null, null, null)),
            CreateTestUserDTO(tutorId2)
        };

        var tutors = new List<Tutor>
        {
            CreateTestTutor(userId: tutorId1),
            CreateTestTutor(userId: tutorId2)
        };

        MockGetUsersByRoleAsync("Tutor", users);

        TutorRepositoryMock
            .Setup(r => r.GetByUserIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(tutors);

        // Act
        var result = await _tutorService.GetAllTutorsAsync();

        // Assert
        result.Items.Should().HaveCount(2);

        var list = result.Items.ToList();

        list.Should().Contain(x => x.UserId == tutorId1);
        list.Should().Contain(x => x.UserId == tutorId2);
    }

    [Fact]
    public async Task GetAllTutorsAsync_WhenNoTutors_ReturnsEmpty()
    {
        // Arrange
        MockGetUsersByRoleAsync("Tutor", new List<UserDTO>());

        TutorRepositoryMock
            .Setup(r => r.GetByUserIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<Tutor>());

        // Act
        var result = await _tutorService.GetAllTutorsAsync();

        // Assert
        result.Items.Should().BeEmpty();
    }
}