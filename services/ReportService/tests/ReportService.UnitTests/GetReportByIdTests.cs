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
public class GetReportByIdTests : BaseTest
{
    [Fact]
    public async Task GetReportByIdAsync_WhenReportExists_ReturnsReportDTO()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var report = new Report
        {
            Id = reportId,
            StudentId = Guid.NewGuid(),
            GeneratedAt = DateTime.UtcNow,
            GPA = 3.5m,
            Status = "Completed",
            ReportDetails = new List<ReportDetail>
            {
                new ReportDetail
                {
                    CourseId = Guid.NewGuid(),
                    Grade = 85m,
                    CourseTitle = "Math 101",
                    Credits = 3,
                    Status = "Completed"
                }
            }
        };
        ReportRepositoryMock.Setup(repo => repo.GetByIdAsync(reportId))
            .Returns(Task.FromResult(report));

        // Act
        var result = await ReportService.GetReportByIdAsync(reportId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(reportId);
        result.GPA.Should().Be(3.5m);
        result.Status.Should().Be("Completed"); // Added
        result.Details.Should().HaveCount(1);
        result.Details.First().CourseTitle.Should().Be("Math 101");
        ReportRepositoryMock.Verify(repo => repo.GetByIdAsync(reportId), Times.Once()); // Added
    }

    [Fact]
    public async Task GetReportByIdAsync_WhenReportDoesNotExist_ReturnsNull()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        ReportRepositoryMock.Setup(repo => repo.GetByIdAsync(reportId))
            .Returns(Task.FromResult<Report>(null));

        // Act
        var result = await ReportService.GetReportByIdAsync(reportId);

        // Assert
        result.Should().BeNull();
        ReportRepositoryMock.Verify(repo => repo.GetByIdAsync(reportId), Times.Once()); // Added
    }
}