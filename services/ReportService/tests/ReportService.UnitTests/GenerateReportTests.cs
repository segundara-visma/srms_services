using FluentAssertions;
using Moq;
using ReportService.Application.DTOs;
using ReportService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ReportService.UnitTests;

[Collection("SequentialTests")]
public class GenerateReportTests : BaseTest
{
    [Fact]
    public async Task GenerateReportAsync_WhenStudentHasGradesAndEnrollments_GeneratesReportSuccessfully()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId1 = Guid.NewGuid();
        var courseId2 = Guid.NewGuid();

        ReportRepositoryMock
            .Setup(repo => repo.GetByStudentIdAsync(studentId))
            .ReturnsAsync((Report?)null);

        GradeServiceClientMock
            .Setup(client => client.GetGradesByStudentAsync(studentId))
            .ReturnsAsync(new List<GradeDTO>
            {
                new GradeDTO(Guid.NewGuid(), studentId, courseId1, 85m, DateTime.UtcNow, null),
                new GradeDTO(Guid.NewGuid(), studentId, courseId2, 95m, DateTime.UtcNow, null)
            });

        EnrollmentServiceClientMock
            .Setup(client => client.CheckEnrollmentAsync(studentId, It.IsAny<Guid>()))
            .ReturnsAsync(true);

        CourseServiceClientMock
            .Setup(client => client.GetCourseDetailsAsync(courseId1))
            .ReturnsAsync(("Math 101", 3));

        CourseServiceClientMock
            .Setup(client => client.GetCourseDetailsAsync(courseId2))
            .ReturnsAsync(("Physics 101", 4));

        ReportRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Report>()))
            .Returns(Task.CompletedTask);

        ReportRepositoryMock
            .Setup(repo => repo.AddDetailAsync(It.IsAny<ReportDetail>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await ReportService.GenerateReportAsync(studentId);

        // Assert
        result.Should().NotBeNull();
        result.StudentId.Should().Be(studentId);
        result.Details.Should().HaveCount(2);

        result.Details.First().CourseTitle.Should().Be("Math 101");
        result.Details.Last().CourseTitle.Should().Be("Physics 101");

        result.GPA.Should().BeApproximately(3.6m, 0.1m);

        ReportRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Report>()), Times.Once);
        ReportRepositoryMock.Verify(r => r.AddDetailAsync(It.IsAny<ReportDetail>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GenerateReportAsync_WhenReportAlreadyExists_ThrowsInvalidOperationException()
    {
        var studentId = Guid.NewGuid();

        ReportRepositoryMock
            .Setup(r => r.GetByStudentIdAsync(studentId))
            .ReturnsAsync(new Report { Id = Guid.NewGuid(), StudentId = studentId });

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ReportService.GenerateReportAsync(studentId));

        ReportRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Report>()), Times.Never);
    }

    [Fact]
    public async Task GenerateReportAsync_WhenStudentHasNoGrades_ThrowsArgumentException()
    {
        var studentId = Guid.NewGuid();

        ReportRepositoryMock
            .Setup(r => r.GetByStudentIdAsync(studentId))
            .ReturnsAsync((Report?)null);

        GradeServiceClientMock
            .Setup(c => c.GetGradesByStudentAsync(studentId))
            .ReturnsAsync(new List<GradeDTO>());

        await Assert.ThrowsAsync<ArgumentException>(
            () => ReportService.GenerateReportAsync(studentId));

        ReportRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Report>()), Times.Never);
    }

    [Fact]
    public async Task GenerateReportAsync_WhenNotEnrolled_ThrowsArgumentException()
    {
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        ReportRepositoryMock
            .Setup(r => r.GetByStudentIdAsync(studentId))
            .ReturnsAsync((Report?)null);

        GradeServiceClientMock
            .Setup(c => c.GetGradesByStudentAsync(studentId))
            .ReturnsAsync(new List<GradeDTO>
            {
                new GradeDTO(Guid.NewGuid(), studentId, courseId, 85m, DateTime.UtcNow, null)
            });

        EnrollmentServiceClientMock
            .Setup(c => c.CheckEnrollmentAsync(studentId, courseId))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<ArgumentException>(
            () => ReportService.GenerateReportAsync(studentId));

        ReportRepositoryMock.Verify(r => r.AddDetailAsync(It.IsAny<ReportDetail>()), Times.Never);
    }

    [Fact]
    public async Task GenerateReportAsync_WhenCourseNotFound_ThrowsArgumentException()
    {
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        ReportRepositoryMock
            .Setup(r => r.GetByStudentIdAsync(studentId))
            .ReturnsAsync((Report?)null);

        GradeServiceClientMock
            .Setup(c => c.GetGradesByStudentAsync(studentId))
            .ReturnsAsync(new List<GradeDTO>
            {
                new GradeDTO(Guid.NewGuid(), studentId, courseId, 85m, DateTime.UtcNow, null)
            });

        EnrollmentServiceClientMock
            .Setup(c => c.CheckEnrollmentAsync(studentId, courseId))
            .ReturnsAsync(true);

        CourseServiceClientMock
            .Setup(c => c.GetCourseDetailsAsync(courseId))
            .ReturnsAsync((ValueTuple<string, int>?)null);

        await Assert.ThrowsAsync<ArgumentException>(
            () => ReportService.GenerateReportAsync(studentId));

        ReportRepositoryMock.Verify(r => r.AddDetailAsync(It.IsAny<ReportDetail>()), Times.Never);
    }
}