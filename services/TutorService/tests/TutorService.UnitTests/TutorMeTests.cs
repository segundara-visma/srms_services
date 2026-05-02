using Moq;
using Moq.Language.Flow;
using FluentAssertions;
using System;
using TutorService.Application.DTOs;
using TutorService.Application.Interfaces;
using TutorService.Application.Services;
using TutorService.Domain.Entities;
using Xunit;

namespace TutorService.UnitTests;

public class TutorMeTests : BaseTest
{
    private readonly ITutorService _tutorService;

    public TutorMeTests()
    {
        _tutorService = new TutorServiceImpl(
            TutorRepositoryMock.Object,
            UserServiceClientMock.Object,
            GradeServiceClientMock.Object);
    }

    [Fact]
    public async Task GetTutorById_ReturnsTutor()
    {
        var userId = Guid.NewGuid();

        var tutor = CreateTestTutor(userId: userId);

        var profile = new ProfileDTO("789 Pine St", "555-7890",
            null, null, null, null, null, null, null, null, null, null, null);

        var user = CreateTestUserDTO(userId, profile);

        MockGetTutorByUserIdAsync(userId, tutor);
        MockGetUserByIdAsync(userId, user);

        var result = await _tutorService.GetTutorByIdAsync(userId);

        result.UserId.Should().Be(userId);
        result.Profile.Should().NotBeNull();
        result.Profile!.Address.Should().Be("789 Pine St");
    }

    [Fact]
    public async Task GetTutorById_WhenNotFound_Throws()
    {
        var userId = Guid.NewGuid();

        MockGetTutorByUserIdAsync(userId, null);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _tutorService.GetTutorByIdAsync(userId));
    }
}