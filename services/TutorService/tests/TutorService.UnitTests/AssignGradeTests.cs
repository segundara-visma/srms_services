using FluentAssertions;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TutorService.Application.Services;
using TutorService.Application.Interfaces;
using TutorService.Application.DTOs;
using Xunit;

namespace TutorService.UnitTests;

public class AssignGradeTests : BaseTest
{
    private readonly ITutorService _tutorService;

    public AssignGradeTests()
    {
        _tutorService = new TutorServiceImpl(TutorRepositoryMock.Object, UserServiceClientMock.Object, GradeServiceClientMock.Object);
    }

    [Fact]
    public async Task AssignGradeAsync_WhenValidInput_ReturnsTrue()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var grade = 85m;
        GradeServiceClientMock.Setup(c => c.AssignGradeAsync(studentId, courseId, grade)).ReturnsAsync(true);

        // Act
        var result = await _tutorService.AssignGradeAsync(studentId, courseId, grade);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AssignGradeAsync_WhenGradeBelowZero_ThrowsException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var grade = -5m;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _tutorService.AssignGradeAsync(studentId, courseId, grade));
    }

    [Fact]
    public async Task AssignGradeAsync_WhenGradeAboveHundred_ThrowsException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var grade = 105m;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _tutorService.AssignGradeAsync(studentId, courseId, grade));
    }
}