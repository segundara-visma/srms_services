using Moq;
using Moq.Language.Flow;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using TutorService.Application.Interfaces;
using TutorService.Application.Services;
using Xunit;

namespace TutorService.UnitTests;

public class AssignGradeTests : BaseTest
{
    private readonly ITutorService _tutorService;

    public AssignGradeTests()
    {
        _tutorService = new TutorServiceImpl(
            TutorRepositoryMock.Object,
            UserServiceClientMock.Object,
            GradeServiceClientMock.Object);
    }

    [Fact]
    public async Task AssignGradeAsync_WhenValidInput_ReturnsTrue()
    {
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var grade = 85m;

        GradeServiceClientMock
            .Setup(c => c.AssignGradeAsync(studentId, courseId, grade))
            .ReturnsAsync(true);

        var result = await _tutorService.AssignGradeAsync(studentId, courseId, grade);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task AssignGradeAsync_WhenInvalidLow_Throws()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _tutorService.AssignGradeAsync(Guid.NewGuid(), Guid.NewGuid(), -1));
    }

    [Fact]
    public async Task AssignGradeAsync_WhenInvalidHigh_Throws()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _tutorService.AssignGradeAsync(Guid.NewGuid(), Guid.NewGuid(), 101));
    }
}