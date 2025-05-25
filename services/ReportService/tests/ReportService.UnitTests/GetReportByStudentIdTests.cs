using FluentAssertions;
using Moq;
using ReportService.Application.DTOs;
using ReportService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ReportService.UnitTests;

[Collection("SequentialTests")]
public class GetReportByStudentIdTests : BaseTest
{
    [Fact]
    public async Task GetReportByStudentIdAsync_WhenReportExists_ReturnsReportDTO()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var report = new Report
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            GeneratedAt = DateTime.UtcNow,
            GPA = 3.8m,
            Status = "Completed",
            ReportDetails = new List<ReportDetail>
            {
                new ReportDetail
                {
                    CourseId = Guid.NewGuid(),
                    Grade = 90m,
                    CourseTitle = "Physics 101",
                    Credits = 4,
                    Status = "Completed"
                }
            }
        };
        ReportRepositoryMock.Setup(repo => repo.GetByStudentIdAsync(studentId))
            .Returns(Task.FromResult(report));

        // Act
        var result = await ReportService.GetReportByStudentIdAsync(studentId);

        // Assert
        result.Should().NotBeNull();
        result.StudentId.Should().Be(studentId);
        result.GPA.Should().Be(3.8m);
        result.Status.Should().Be("Completed"); // Added
        result.Details.Should().HaveCount(1);
        result.Details.First().CourseTitle.Should().Be("Physics 101");
        ReportRepositoryMock.Verify(repo => repo.GetByStudentIdAsync(studentId), Times.Once()); // Added
    }

    [Fact]
    public async Task GetReportByStudentIdAsync_WhenReportDoesNotExist_ReturnsNull()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        ReportRepositoryMock.Setup(repo => repo.GetByStudentIdAsync(studentId))
            .Returns(Task.FromResult<Report>(null));

        // Act
        var result = await ReportService.GetReportByStudentIdAsync(studentId);

        // Assert
        result.Should().BeNull();
        ReportRepositoryMock.Verify(repo => repo.GetByStudentIdAsync(studentId), Times.Once()); // Added
    }
}