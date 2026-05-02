using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorService.Application.Interfaces;
using TutorService.Application.Services;
using TutorService.Domain.Entities;
using Xunit;

namespace TutorService.UnitTests;

public class AssignCourseToTutorTests : BaseTest
{
    private readonly ITutorService _tutorService;

    public AssignCourseToTutorTests()
    {
        _tutorService = new TutorServiceImpl(
            TutorRepositoryMock.Object,
            UserServiceClientMock.Object,
            GradeServiceClientMock.Object);
    }

    [Fact]
    public async Task AssignCourseToTutorAsync_WhenValid_AddsCourse()
    {
        var tutorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var tutor = CreateTestTutor(userId: tutorId);

        MockGetTutorByUserIdAsync(tutorId, tutor);

        TutorRepositoryMock
            .Setup(r => r.GetCoursesByTutorIdAsync(tutorId))
            .ReturnsAsync(new List<TutorCourse>());

        TutorRepositoryMock
            .Setup(r => r.AddTutorCourseAsync(It.IsAny<TutorCourse>()))
            .Returns(Task.CompletedTask);

        await _tutorService.AssignCourseToTutorAsync(tutorId, courseId);

        TutorRepositoryMock.Verify(r =>
            r.AddTutorCourseAsync(It.Is<TutorCourse>(tc =>
                tc.TutorId == tutorId &&
                tc.CourseId == courseId)),
            Times.Once);
    }

    [Fact]
    public async Task AssignCourseToTutorAsync_WhenExists_Throws()
    {
        var tutorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        MockGetTutorByUserIdAsync(tutorId, CreateTestTutor(tutorId, tutorId));

        TutorRepositoryMock
            .Setup(r => r.GetCoursesByTutorIdAsync(tutorId))
            .ReturnsAsync(new List<TutorCourse>
            {
                new TutorCourse { TutorId = tutorId, CourseId = courseId }
            });

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _tutorService.AssignCourseToTutorAsync(tutorId, courseId));
    }
}