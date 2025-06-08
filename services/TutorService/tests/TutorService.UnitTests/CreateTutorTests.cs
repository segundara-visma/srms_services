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

public class CreateTutorTests : BaseTest
{
    private readonly ITutorService _tutorService;

    public CreateTutorTests()
    {
        _tutorService = new TutorServiceImpl(TutorRepositoryMock.Object, UserServiceClientMock.Object, GradeServiceClientMock.Object);
    }

    [Fact]
    public async Task CreateTutorAsync_WhenValidInput_CreatesTutor()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateTestUserDTO(userId);
        UserServiceClientMock.Setup(c => c.GetUserByIdAsync(userId)).ReturnsAsync(user);
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Tutor)null);
        TutorRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Tutor>()))
            .Callback<Tutor>(t => t.Id = Guid.NewGuid()) // Simulate Id assignment
            .Returns(Task.CompletedTask);

        // Act
        await _tutorService.CreateTutorAsync(userId);

        // Assert
        TutorRepositoryMock.Verify(r => r.AddAsync(It.Is<Tutor>(t => t.UserId == userId && t.Id != Guid.Empty)), Times.Once());
    }

    [Fact]
    public async Task CreateTutorAsync_WhenTutorExists_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateTestUserDTO(userId);
        var existingTutor = CreateTestTutor(userId: userId);
        UserServiceClientMock.Setup(c => c.GetUserByIdAsync(userId)).ReturnsAsync(user);
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existingTutor);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tutorService.CreateTutorAsync(userId));
    }

    [Fact]
    public async Task CreateTutorAsync_WhenUserIdEmpty_ThrowsException()
    {
        // Arrange
        var userId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _tutorService.CreateTutorAsync(userId));
    }
}