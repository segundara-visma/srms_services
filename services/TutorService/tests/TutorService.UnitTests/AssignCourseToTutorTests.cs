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
        var userId = Guid.NewGuid();           // This is what comes into the method
        var courseId = Guid.NewGuid();
        var tutor = CreateTestTutor(userId: userId);   // Make sure tutor.Id is different from userId

        // Mock getting the tutor by UserId
        MockGetTutorByUserIdAsync(userId, tutor);

        // IMPORTANT: Mock using tutor.Id (the Tutor entity's Id), not userId
        TutorRepositoryMock
            .Setup(r => r.GetCoursesByTutorIdAsync(tutor.Id))
            .ReturnsAsync(new List<TutorCourse>());

        TutorRepositoryMock
            .Setup(r => r.AddTutorCourseAsync(It.IsAny<TutorCourse>()))
            .Returns(Task.CompletedTask);

        await _tutorService.AssignCourseToTutorAsync(userId, courseId);

        // Verify with tutor.Id
        TutorRepositoryMock.Verify(r =>
            r.AddTutorCourseAsync(It.Is<TutorCourse>(tc =>
                tc.TutorId == tutor.Id &&           // Use tutor.Id here
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