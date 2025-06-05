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
        var tutorId = Guid.NewGuid();
        var users = new List<UserDTO>
        {
            new UserDTO(tutorId, "Tutor", "One", "tutor1@example.com", "123", "Tutor") // Use primary constructor
        };
        var tutors = new List<Tutor>
        {
            new Tutor { Id = Guid.NewGuid(), UserId = tutorId }
        };
        UserServiceClientMock.Setup(c => c.GetUsersByRoleAsync("Tutor")).ReturnsAsync(users);
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(tutorId)).ReturnsAsync(tutors[0]);

        // Act
        var result = await _tutorService.GetAllTutorsAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(t => t.Email == "tutor1@example.com");
    }
}