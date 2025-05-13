using EnrollmentService.Domain.Entities;
using EnrollmentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;

namespace EnrollmentService.UnitTests;

public class GetEnrollmentTests : BaseTest
{
    [Fact]
    public async Task GetEnrollmentByIdAsync_ValidId_ReturnsEnrollment()
    {
        // Arrange
        var enrollmentId = Guid.NewGuid();
        var enrollment = CreateSampleEnrollment(Guid.NewGuid(), Guid.NewGuid());
        enrollment.Id = enrollmentId;
        MockRepository.Setup(x => x.GetByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);

        // Act
        var result = await Service.GetEnrollmentByIdAsync(enrollmentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(enrollmentId, result.Id);
        Assert.Equal(enrollment.StudentId, result.StudentId);
        Assert.Equal(enrollment.CourseId, result.CourseId);
        Assert.Equal(enrollment.EnrollmentDate, result.EnrollmentDate);
        Assert.Equal(enrollment.Status.ToString(), result.Status); // Compare string representation
        Assert.Equal(enrollment.PaymentAmount, result.PaymentAmount);
        MockRepository.Verify(x => x.GetByIdAsync(enrollmentId), Times.Once());
    }

    [Fact]
    public async Task GetEnrollmentsByStudentAsync_ValidStudent_ReturnsEnrollments()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var enrollments = new List<Enrollment>
        {
            CreateSampleEnrollment(studentId, Guid.NewGuid())
        };
        MockRepository.Setup(x => x.GetByStudentIdAsync(studentId))
            .ReturnsAsync(enrollments);
        MockUserClient.Setup(x => x.GetUserByIdAsync(studentId))
            .ReturnsAsync(CreateSampleUser(studentId));

        // Act
        var result = await Service.GetEnrollmentsByStudentAsync(studentId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(studentId, result.First().StudentId);
        Assert.Equal(enrollments[0].EnrollmentDate, result.First().EnrollmentDate);
        Assert.Equal(enrollments[0].Status.ToString(), result.First().Status); // Compare string representation
        MockRepository.Verify(x => x.GetByStudentIdAsync(studentId), Times.Once());
    }

    [Fact]
    public async Task GetEnrollmentsByCourseAsync_ValidCourse_ReturnsEnrollments()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var enrollments = new List<Enrollment>
        {
            CreateSampleEnrollment(Guid.NewGuid(), courseId)
        };
        MockRepository.Setup(x => x.GetByCourseIdAsync(courseId))
            .ReturnsAsync(enrollments);
        MockCourseClient.Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(CreateSampleCourse(courseId));

        // Act
        var result = await Service.GetEnrollmentsByCourseAsync(courseId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(courseId, result.First().CourseId);
        Assert.Equal(enrollments[0].EnrollmentDate, result.First().EnrollmentDate);
        Assert.Equal(enrollments[0].Status.ToString(), result.First().Status); // Compare string representation
        MockRepository.Verify(x => x.GetByCourseIdAsync(courseId), Times.Once());
    }
}