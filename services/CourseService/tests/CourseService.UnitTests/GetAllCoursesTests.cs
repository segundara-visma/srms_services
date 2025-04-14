using CourseService.Application.DTOs;
using CourseService.Domain.Entities;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseService.UnitTests;

public class GetAllCoursesTests : BaseTest
{
    [Fact]
    public async Task GetAllCourses_MultipleCourses_ReturnsAllCourses()
    {
        // Arrange
        var course1 = CreateTestCourse();
        var course2 = CreateTestCourse();
        var courses = new List<Course> { course1, course2 };
        MockGetAllCoursesAsync(courses);

        // Act
        var result = await _courseService.GetAllCoursesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Id == course1.Id && c.Name == course1.Name);
        Assert.Contains(result, c => c.Id == course2.Id && c.Name == course2.Name);
    }

    [Fact]
    public async Task GetAllCourses_NoCourses_ReturnsEmptyList()
    {
        // Arrange
        MockGetAllCoursesAsync(new List<Course>());

        // Act
        var result = await _courseService.GetAllCoursesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}