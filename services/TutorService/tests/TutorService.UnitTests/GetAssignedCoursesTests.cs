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

public class GetAssignedCoursesTests : BaseTest
{
    private readonly ITutorService _tutorService;

    public GetAssignedCoursesTests()
    {
        _tutorService = new TutorServiceImpl(TutorRepositoryMock.Object, UserServiceClientMock.Object, GradeServiceClientMock.Object);
    }

    [Fact]
    public async Task GetAssignedCoursesAsync_WhenTutorHasCourses_ReturnsCourseIds()
    {
        // Arrange
        var tutorId = Guid.NewGuid();
        var tutor = CreateTestTutor(userId: tutorId);
        var courses = new List<TutorCourse>
        {
            new TutorCourse { Id = Guid.NewGuid(), TutorId = tutorId, CourseId = Guid.NewGuid(), AssignmentDate = DateTime.UtcNow },
            new TutorCourse { Id = Guid.NewGuid(), TutorId = tutorId, CourseId = Guid.NewGuid(), AssignmentDate = DateTime.UtcNow }
        };
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(tutorId)).ReturnsAsync(tutor);
        TutorRepositoryMock.Setup(r => r.GetCoursesByTutorIdAsync(tutorId)).ReturnsAsync(courses);

        // Act
        var result = await _tutorService.GetAssignedCoursesAsync(tutorId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(courses[0].CourseId);
        result.Should().Contain(courses[1].CourseId);
        result.Should().OnlyHaveUniqueItems(); // Ensure no duplicates
    }

    [Fact]
    public async Task GetAssignedCoursesAsync_WhenTutorNotFound_ThrowsException()
    {
        // Arrange
        var tutorId = Guid.NewGuid();
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(tutorId)).ReturnsAsync((Tutor)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _tutorService.GetAssignedCoursesAsync(tutorId));
    }
}