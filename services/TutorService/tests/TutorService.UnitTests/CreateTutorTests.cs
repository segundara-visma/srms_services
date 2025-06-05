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
        var user = new UserDTO(userId, "John", "Doe", "john.doe@example.com", "123", "Tutor");
        UserServiceClientMock.Setup(c => c.GetUserByIdAsync(userId)).ReturnsAsync(user);
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Tutor)null);
        TutorRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Tutor>())).Returns(Task.CompletedTask);

        // Act
        await _tutorService.CreateTutorAsync(userId);

        // Assert
        TutorRepositoryMock.Verify(r => r.AddAsync(It.Is<Tutor>(t => t.UserId == userId)), Times.Once());
    }

    [Fact]
    public async Task CreateTutorAsync_WhenTutorExists_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserDTO(userId, "John", "Doe", "john.doe@example.com", "123", "Tutor");
        var existingTutor = new Tutor { Id = Guid.NewGuid(), UserId = userId };
        UserServiceClientMock.Setup(c => c.GetUserByIdAsync(userId)).ReturnsAsync(user);
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existingTutor);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tutorService.CreateTutorAsync(userId));
    }
}