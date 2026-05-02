using Moq;
using Moq.Language.Flow;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using TutorService.Application.Interfaces;
using TutorService.Application.Services;
using TutorService.Domain.Entities;
using Xunit;

namespace TutorService.UnitTests;

public class GetTutorByIdTests : BaseTest
{
    private readonly ITutorService _tutorService;

    public GetTutorByIdTests()
    {
        _tutorService = new TutorServiceImpl(
            TutorRepositoryMock.Object,
            UserServiceClientMock.Object,
            GradeServiceClientMock.Object);
    }

    [Fact]
    public async Task ReturnsTutor_WhenExists()
    {
        var id = Guid.NewGuid();

        var tutor = CreateTestTutor(userId: id);
        var user = CreateTestUserDTO(id);

        MockGetTutorByUserIdAsync(id, tutor);
        MockGetUserByIdAsync(id, user);

        var result = await _tutorService.GetTutorByIdAsync(id);

        result.Should().NotBeNull();
        result.UserId.Should().Be(id);
    }

    [Fact]
    public async Task Throws_WhenTutorMissing()
    {
        var id = Guid.NewGuid();

        MockGetTutorByUserIdAsync(id, null);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _tutorService.GetTutorByIdAsync(id));
    }
}