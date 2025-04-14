using CourseService.Application.DTOs;
using CourseService.Domain.Entities;
using Moq;
using Xunit;
using System.Threading.Tasks;

namespace CourseService.UnitTests;

public class GetCourseTests : BaseTest
{
    [Fact]
    public async Task GetCourseById_ExistingId_ReturnsCourse()
    {
        // Arrange
        var course = CreateTestCourse();
        MockGetByIdAsync(course);

        // Act
        var result = await _courseService.GetCourseByIdAsync(course.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(course.Id, result.Id);
        Assert.Equal(course.Name, result.Name);
        Assert.Equal(course.Code, result.Code);
        Assert.Equal(course.MaxStudents, result.MaxStudents);
    }

    [Fact]
    public async Task GetCourseById_NonExistingId_ReturnsNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(nonExistingId)).ReturnsAsync((Course)null);

        // Act
        var result = await _courseService.GetCourseByIdAsync(nonExistingId);

        // Assert
        Assert.Null(result);
    }
}