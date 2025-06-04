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
        var tutor = new Tutor { Id = Guid.NewGuid(), UserId = tutorId };
        TutorRepositoryMock.Setup(r => r.GetByUserIdAsync(tutorId)).ReturnsAsync(tutor);
        TutorRepositoryMock.Setup(r => r.GetCoursesByTutorIdAsync(tutorId)).ReturnsAsync(new List<TutorCourse>());
        TutorRepositoryMock.Setup(r => r.AddTutorCourseAsync(It.IsAny<TutorCourse>())).Returns(Task.CompletedTask);

        // Act
        await _tutorService.AssignCourseToTutorAsync(tutorId, courseId);

        // Assert
        TutorRepositoryMock.Verify(r => r.AddTutorCourseAsync(It.Is<TutorCourse>(tc => tc.TutorId == tutorId && tc.CourseId == courseId)), Times.Once());
    }
}