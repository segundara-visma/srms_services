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
        var tutor = new Tutor { Id = Guid.NewGuid(), UserId = tutorId };
        var user = new UserDTO(tutorId, "Tutor", "One", "tutor1@example.com", "123", "Tutor"); // Use primary constructor
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(tutorId)).ReturnsAsync(tutor);
        UserServiceClientMock.Setup(c => c.GetUserByIdAsync(tutorId)).ReturnsAsync(user);

        // Act
        var result = await _tutorService.GetTutorByIdAsync(tutorId);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(tutorId);
        result.Email.Should().Be("tutor1@example.com");
    }
}