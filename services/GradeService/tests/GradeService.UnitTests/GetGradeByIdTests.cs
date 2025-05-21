using FluentAssertions;
using GradeService.Application.DTOs;
using GradeService.Application.Services;
using GradeService.Domain.Entities;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GradeService.UnitTests;

public class GetGradeByIdTests : BaseTest
{
    [Fact]
    public async Task GetGradeByIdAsync_WhenGradeExists_ReturnsGradeDTO()
    {
        // Arrange
        var gradeId = Guid.NewGuid();
        var grade = new Grade
        {
            Id = gradeId,
            StudentId = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            GradeValue = 85.5m,
            DateGraded = DateTime.UtcNow,
            Comments = "Good work"
        };
        GradeRepositoryMock.Setup(repo => repo.GetByIdAsync(gradeId)).ReturnsAsync(grade);

        // Act
        var result = await GradeService.GetGradeByIdAsync(gradeId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(gradeId);
        result.GradeValue.Should().Be(85.5m);
        result.Comments.Should().Be("Good work");
    }

    [Fact]
    public async Task GetGradeByIdAsync_WhenGradeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var gradeId = Guid.NewGuid();
        GradeRepositoryMock.Setup(repo => repo.GetByIdAsync(gradeId)).ReturnsAsync((Grade)null);

        // Act
        var result = await GradeService.GetGradeByIdAsync(gradeId);

        // Assert
        result.Should().BeNull();
    }
}