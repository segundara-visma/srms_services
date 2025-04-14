using CourseService.Application.DTOs;
using CourseService.Domain.Entities;
using Moq;
using Xunit;
using System.Threading.Tasks;

namespace CourseService.UnitTests;

public class CreateCourseTests : BaseTest
{
    [Fact]
    public async Task CreateCourse_ValidInput_SavesToDatabase()
    {
        // Arrange
        MockAddCourseAsync();
        var dto = CreateTestCreateCourseDTO();

        // Act
        var courseId = await _courseService.CreateCourseAsync(dto);

        // Assert
        _courseRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Course>(
            c => c.Name == dto.Name && c.Code == dto.Code && c.MaxStudents == dto.MaxStudents)),
            Times.Once());
        Assert.NotEqual(Guid.Empty, courseId);
    }

    [Fact]
    public async Task CreateCourse_NegativeMaxStudents_ThrowsArgumentException()
    {
        // Arrange
        var dto = new CreateCourseDTO("Mathematics 101", "MATH101", -1);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _courseService.CreateCourseAsync(dto));
    }
}