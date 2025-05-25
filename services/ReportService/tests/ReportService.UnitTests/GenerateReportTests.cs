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

[Collection("SequentialTests")] // Added to serialize test execution
public class GenerateReportTests : BaseTest
{
    [Fact]
    public async Task GenerateReportAsync_WhenStudentHasGradesAndEnrollments_GeneratesReportSuccessfully()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId1 = Guid.NewGuid();
        var courseId2 = Guid.NewGuid();
        ReportRepositoryMock.Setup(repo => repo.GetByStudentIdAsync(studentId)).ReturnsAsync((Report)null);
        GradeServiceClientMock.Setup(client => client.GetGradesByStudentAsync(studentId))
            .Returns(Task.FromResult<IEnumerable<GradeDTO>>(new List<GradeDTO>
            {
                new GradeDTO { StudentId = studentId, CourseId = courseId1, GradeValue = 85m },
                new GradeDTO { StudentId = studentId, CourseId = courseId2, GradeValue = 95m }
            }.AsEnumerable()));
        EnrollmentServiceClientMock.Setup(client => client.CheckEnrollmentAsync(studentId, It.IsAny<Guid>())).ReturnsAsync(true);
        CourseServiceClientMock.Setup(client => client.GetCourseDetailsAsync(courseId1))
            .ReturnsAsync(("Math 101", 3));
        CourseServiceClientMock.Setup(client => client.GetCourseDetailsAsync(courseId2))
            .ReturnsAsync(("Physics 101", 4));
        ReportRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Report>())).Returns(Task.FromResult(Task.CompletedTask));
        ReportRepositoryMock.Setup(repo => repo.AddDetailAsync(It.IsAny<ReportDetail>())).Returns(Task.FromResult(Task.CompletedTask));

        // Act
        var result = await ReportService.GenerateReportAsync(studentId);

        // Assert
        result.Should().NotBeNull();
        result.StudentId.Should().Be(studentId);
        result.Details.Should().HaveCount(2);
        result.Details.First().CourseTitle.Should().Be("Math 101");
        result.Details.Last().CourseTitle.Should().Be("Physics 101");
        result.GPA.Should().BeApproximately(3.6m, 0.1m); // (85 + 95) / 50 = 3.6
        ReportRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Report>()), Times.Once());
        ReportRepositoryMock.Verify(repo => repo.AddDetailAsync(It.IsAny<ReportDetail>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GenerateReportAsync_WhenReportAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        ReportRepositoryMock.Setup(repo => repo.GetByStudentIdAsync(studentId))
            .ReturnsAsync(new Report { Id = Guid.NewGuid(), StudentId = studentId });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => ReportService.GenerateReportAsync(studentId));
        ReportRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Report>()), Times.Never());
    }

    [Fact]
    public async Task GenerateReportAsync_WhenStudentHasNoGrades_ThrowsArgumentException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        ReportRepositoryMock.Setup(repo => repo.GetByStudentIdAsync(studentId)).ReturnsAsync((Report)null);
        GradeServiceClientMock.Setup(client => client.GetGradesByStudentAsync(studentId))
            .Returns(Task.FromResult<IEnumerable<GradeDTO>>(new List<GradeDTO>().AsEnumerable()));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => ReportService.GenerateReportAsync(studentId));
        ReportRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Report>()), Times.Never());
    }

    [Fact]
    public async Task GenerateReportAsync_WhenNotEnrolledInCourse_SkipsCourseInDetails()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        ReportRepositoryMock.Setup(repo => repo.GetByStudentIdAsync(studentId)).ReturnsAsync((Report)null);
        GradeServiceClientMock.Setup(client => client.GetGradesByStudentAsync(studentId))
            .Returns(Task.FromResult<IEnumerable<GradeDTO>>(new List<GradeDTO>
            {
                new GradeDTO { StudentId = studentId, CourseId = courseId, GradeValue = 85m }
            }.AsEnumerable()));
        EnrollmentServiceClientMock.Setup(client => client.CheckEnrollmentAsync(studentId, courseId)).ReturnsAsync(false);
        CourseServiceClientMock.Setup(client => client.GetCourseDetailsAsync(courseId))
            .ReturnsAsync(("Math 101", 3));

        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => ReportService.GenerateReportAsync(studentId));
        ReportRepositoryMock.Verify(repo => repo.AddDetailAsync(It.IsAny<ReportDetail>()), Times.Never());
    }

    [Fact]
    public async Task GenerateReportAsync_WhenCourseDetailsNotFound_SkipsCourseInDetails()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        ReportRepositoryMock.Setup(repo => repo.GetByStudentIdAsync(studentId)).ReturnsAsync((Report)null);
        GradeServiceClientMock.Setup(client => client.GetGradesByStudentAsync(studentId))
            .Returns(Task.FromResult<IEnumerable<GradeDTO>>(new List<GradeDTO>
            {
                new GradeDTO { StudentId = studentId, CourseId = courseId, GradeValue = 85m }
            }.AsEnumerable()));
        EnrollmentServiceClientMock.Setup(client => client.CheckEnrollmentAsync(studentId, courseId)).ReturnsAsync(true);
        CourseServiceClientMock.Setup(client => client.GetCourseDetailsAsync(courseId))
            .ReturnsAsync((ValueTuple<string, int>?)null);

        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => ReportService.GenerateReportAsync(studentId));
        ReportRepositoryMock.Verify(repo => repo.AddDetailAsync(It.IsAny<ReportDetail>()), Times.Never());
    }
}