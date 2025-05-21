using FluentAssertions;
using GradeService.Application.DTOs;
using GradeService.Application.Services;
using GradeService.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GradeService.UnitTests;

public class GetAllGradesTests : BaseTest
{
    [Fact]
    public async Task GetAllGradesAsync_WhenGradesExist_ReturnsListOfGradeDTOs()
    {
        // Arrange
        var grades = new List<Grade>
        {
            new Grade
            {
                Id = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GradeValue = 90.0m,
                DateGraded = DateTime.UtcNow,
                Comments = "Excellent"
            },
            new Grade
            {
                Id = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GradeValue = 75.5m,
                DateGraded = DateTime.UtcNow,
                Comments = "Needs improvement"
            }
        };
        GradeRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(grades);

        // Act
        var result = await GradeService.GetAllGradesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().GradeValue.Should().Be(90.0m);
        result.Last().GradeValue.Should().Be(75.5m);
    }

    [Fact]
    public async Task GetAllGradesAsync_WhenNoGradesExist_ReturnsEmptyList()
    {
        // Arrange
        GradeRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Grade>());

        // Act
        var result = await GradeService.GetAllGradesAsync();

        // Assert
        result.Should().BeEmpty();
    }
}