using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using AdminService.Application.Interfaces;
using AdminService.Application.Services;
using Xunit;

namespace AdminService.UnitTests;

public class AssignCourseToTutorTests : BaseTest
{
    private readonly IAdminService _adminService;

    public AssignCourseToTutorTests()
    {
        _adminService = new AdminServiceImpl(
            UserServiceClientMock.Object,
            TutorServiceClientMock.Object,
            StudentServiceClientMock.Object,
            GradeServiceClientMock.Object,
            CourseServiceClientMock.Object,
            EnrollmentServiceClientMock.Object);
    }

    [Fact]
    public async Task AssignCourseToTutorAsync_WhenValidInput_AssignsCourse()
    {
        // Arrange
        var tutorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        TutorServiceClientMock.Setup(c => c.AssignCourseToTutorAsync(tutorId, courseId)).Returns(Task.CompletedTask);

        // Act
        await _adminService.AssignCourseToTutorAsync(tutorId, courseId);

        // Assert
        TutorServiceClientMock.Verify(c => c.AssignCourseToTutorAsync(tutorId, courseId), Times.Once());
    }

    [Fact]
    public async Task AssignCourseToTutorAsync_WhenInvalidTutorId_ThrowsArgumentException()
    {
        // Arrange
        var tutorId = Guid.Empty;
        var courseId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _adminService.AssignCourseToTutorAsync(tutorId, courseId));
    }
}