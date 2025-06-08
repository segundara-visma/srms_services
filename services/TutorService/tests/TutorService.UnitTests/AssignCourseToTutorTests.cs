using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using TutorService.Domain.Entities;
using TutorService.Application.Services;
using TutorService.Application.Interfaces;
using Xunit;

namespace TutorService.UnitTests;

public class AssignCourseToTutorTests : BaseTest
{
    private readonly ITutorService _tutorService;

    public AssignCourseToTutorTests()
    {
        _tutorService = new TutorServiceImpl(TutorRepositoryMock.Object, UserServiceClientMock.Object, GradeServiceClientMock.Object);
    }

    [Fact]
    public async Task AssignCourseToTutorAsync_WhenValidInput_AddsCourse()
    {
        // Arrange
        var tutorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var tutor = CreateTestTutor(userId: tutorId);
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(tutorId)).ReturnsAsync(tutor);
        TutorRepositoryMock.Setup(r => r.GetCoursesByTutorIdAsync(tutorId)).ReturnsAsync(new List<TutorCourse>());
        TutorRepositoryMock.Setup(r => r.AddTutorCourseAsync(It.IsAny<TutorCourse>()))
            .Callback<TutorCourse>(tc =>
            {
                tc.Id = Guid.NewGuid(); // Simulate Id assignment
                tc.AssignmentDate = DateTime.UtcNow; // Simulate service behavior
            })
            .Returns(Task.CompletedTask);

        // Act
        await _tutorService.AssignCourseToTutorAsync(tutorId, courseId);

        // Assert
        TutorRepositoryMock.Verify(r => r.AddTutorCourseAsync(It.Is<TutorCourse>(
            tc => tc.TutorId == tutorId &&
                  tc.CourseId == courseId &&
                  tc.AssignmentDate != default && // Check if set (not DateTime.MinValue)
                  tc.Id != Guid.Empty)), Times.Once());
    }

    [Fact]
    public async Task AssignCourseToTutorAsync_WhenTutorHasCourse_ThrowsException()
    {
        // Arrange
        var tutorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var tutor = CreateTestTutor(userId: tutorId);
        var existingCourse = new List<TutorCourse> { new TutorCourse { TutorId = tutorId, CourseId = courseId } };
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(tutorId)).ReturnsAsync(tutor);
        TutorRepositoryMock.Setup(r => r.GetCoursesByTutorIdAsync(tutorId)).ReturnsAsync(existingCourse);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _tutorService.AssignCourseToTutorAsync(tutorId, courseId));
    }
}