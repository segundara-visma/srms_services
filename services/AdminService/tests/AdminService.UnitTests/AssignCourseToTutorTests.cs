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
        var tutorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        MockAssignCourseToTutorAsync(tutorId, courseId);

        await _adminService.AssignCourseToTutorAsync(tutorId, courseId);

        TutorServiceClientMock.Verify(c => c.AssignCourseToTutorAsync(tutorId, courseId), Times.Once());
    }

    [Fact]
    public async Task AssignCourseToTutorAsync_WhenInvalidTutorId_ThrowsArgumentException()
    {
        var tutorId = Guid.Empty;
        var courseId = Guid.NewGuid();

        await Assert.ThrowsAsync<ArgumentException>(() => _adminService.AssignCourseToTutorAsync(tutorId, courseId));
    }

    [Fact]
    public async Task AssignCourseToTutorAsync_WhenInvalidCourseId_ThrowsArgumentException()
    {
        var tutorId = Guid.NewGuid();
        var courseId = Guid.Empty;

        await Assert.ThrowsAsync<ArgumentException>(() => _adminService.AssignCourseToTutorAsync(tutorId, courseId));
    }
}