using FluentAssertions;
using GradeService.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
                GradedAt = DateTime.UtcNow,
                Comments = "Excellent"
            },
            new Grade
            {
                Id = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                GradeValue = 75.5m,
                GradedAt = DateTime.UtcNow,
                Comments = "Needs improvement"
            }
        };

        GradeRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(grades);

        // Act
        var result = (await GradeService.GetAllGradesAsync()).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.First().GradeValue.Should().Be(90.0m);
        result.Last().GradeValue.Should().Be(75.5m);
        result.First().GradedAt.Should().Be(grades[0].GradedAt);
        result.Last().GradedAt.Should().Be(grades[1].GradedAt);
    }

    [Fact]
    public async Task GetAllGradesAsync_WhenNoGradesExist_ReturnsEmptyList()
    {
        // Arrange
        GradeRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Grade>());

        // Act
        var result = await GradeService.GetAllGradesAsync();

        // Assert
        result.Should().BeEmpty();
    }
}