using EnrollmentService.Domain.Entities;
using EnrollmentService.Application.DTOs;
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;

namespace EnrollmentService.UnitTests;

public class EnrollStudentAsyncTests : BaseTest
{
    [Fact]
    public async Task EnrollStudentAsync_SuccessfulEnrollment_ReturnsNoException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        MockUserClient.Setup(x => x.GetUserByIdAsync(studentId))
            .ReturnsAsync(CreateSampleUser(studentId));
        MockCourseClient.Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(CreateSampleCourse(courseId));
        MockRepository.Setup(x => x.AddAsync(It.IsAny<Enrollment>()))
            .Returns(Task.CompletedTask);

        // Act
        var exception = await Record.ExceptionAsync(() => Service.EnrollStudentAsync(studentId, courseId));

        // Assert
        Assert.Null(exception);
        MockRepository.Verify(x => x.AddAsync(It.Is<Enrollment>(e =>
            e.StudentId == studentId &&
            e.CourseId == courseId &&
            e.Status == EnrollmentStatus.Enrolled)), Times.Once());
    }

    [Fact]
    public async Task EnrollStudentAsync_InvalidStudent_ThrowsArgumentException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        MockUserClient.Setup(x => x.GetUserByIdAsync(studentId))
            .ReturnsAsync((UserDTO)null!); // Explicitly nullable override
        MockCourseClient.Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(CreateSampleCourse(courseId));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Service.EnrollStudentAsync(studentId, courseId));
        Assert.Contains("Invalid student", exception.Message);
    }

    [Fact]
    public async Task EnrollStudentAsync_InvalidCourse_ThrowsArgumentException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        MockUserClient.Setup(x => x.GetUserByIdAsync(studentId))
            .ReturnsAsync(CreateSampleUser(studentId));
        MockCourseClient.Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync((CourseDTO)null!); // Explicitly nullable override

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Service.EnrollStudentAsync(studentId, courseId));
        Assert.Contains("Invalid course", exception.Message);
    }
}