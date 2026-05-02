using CourseService.Domain.Entities;
using Moq;
using Xunit;
using System;

namespace CourseService.UnitTests;

public class UpdateCourseTests : BaseTest
{
    [Fact]
    public async Task UpdateCourse_ValidInput_UpdatesDatabase()
    {
        // Arrange
        var course = CreateTestCourse();
        MockGetByIdAsync(course);
        MockUpdateCourseAsync();

        var dto = CreateTestUpdateCourseDTO();

        // Act
        await _courseService.UpdateCourseAsync(course.Id, dto);

        // Assert
        _courseRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Course>(
            c => c.Id == course.Id &&
                 c.Name == dto.Name &&
                 c.Code == dto.Code &&
                 c.MaxStudents == dto.MaxStudents)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateCourse_NonExistingId_ThrowsException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _courseRepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Course?)null);

        var dto = CreateTestUpdateCourseDTO();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _courseService.UpdateCourseAsync(id, dto));
    }
}