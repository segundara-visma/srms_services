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

        var paginatedResult = new PaginatedResult<Enrollment>
        {
            Items = enrollments,
            TotalCount = enrollments.Count
        };

        MockRepository
            .Setup(x => x.GetByStudentIdAsync(studentId, 1, 10))
            .ReturnsAsync(paginatedResult);

        MockUserClient
            .Setup(x => x.GetUserByIdAsync(studentId))
            .ReturnsAsync(CreateSampleUser(studentId));

        // Act
        var result = await Service.GetEnrollmentsByStudentAsync(studentId, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);

        var item = result.Items.First();

        Assert.Equal(studentId, item.StudentId);
        Assert.Equal(enrollments[0].EnrollmentDate, item.EnrollmentDate);
        Assert.Equal(enrollments[0].Status.ToString(), item.Status);

        MockRepository.Verify(x => x.GetByStudentIdAsync(studentId, 1, 10), Times.Once());
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

        var paginatedResult = new PaginatedResult<Enrollment>
        {
            Items = enrollments,
            TotalCount = enrollments.Count
        };

        MockRepository
            .Setup(x => x.GetByCourseIdAsync(courseId, 1, 10))
            .ReturnsAsync(paginatedResult);

        MockCourseClient
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(CreateSampleCourse(courseId));

        // Act
        var result = await Service.GetEnrollmentsByCourseAsync(courseId, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);

        var item = result.Items.First();

        Assert.Equal(courseId, item.CourseId);
        Assert.Equal(enrollments[0].EnrollmentDate, item.EnrollmentDate);
        Assert.Equal(enrollments[0].Status.ToString(), item.Status);

        MockRepository.Verify(x => x.GetByCourseIdAsync(courseId, 1, 10), Times.Once());
    }
}