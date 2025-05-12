using StudentService.Application.DTOs;
using StudentService.Domain.Entities;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentService.UnitTests;

public class GetStudentCoursesTests : BaseTest
{
    [Fact]
    public async Task GetStudentCourses_StudentWithCourses_ReturnsCourses()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var student = CreateTestStudent(userId: userId);
        var courseId1 = Guid.NewGuid();
        var courseId2 = Guid.NewGuid();
        student.Enroll(courseId1);
        student.Enroll(courseId2);

        var userDTO = CreateTestUserDTO(userId);
        var courseDTO1 = CreateTestCourseDTO(courseId1);
        var courseDTO2 = CreateTestCourseDTO(courseId2);

        MockGetStudentByUserIdAsync(userId, student);
        MockGetUserByIdAsync(userId, userDTO);
        MockGetCourseByIdAsync(courseId1, courseDTO1);
        MockGetCourseByIdAsync(courseId2, courseDTO2);

        // Act
        var result = await _studentService.GetStudentCoursesAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Id == courseId1);
        Assert.Contains(result, c => c.Id == courseId2);
    }

    [Fact]
    public async Task GetStudentCourses_NonExistingStudent_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _studentRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync((Student)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _studentService.GetStudentCoursesAsync(userId));
    }
}