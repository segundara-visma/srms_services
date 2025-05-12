using StudentService.Domain.Entities;
using Moq;
using Xunit;
using System.Threading.Tasks;
using StudentService.Application.DTOs;

namespace StudentService.UnitTests;

public class EnrollStudentTests : BaseTest
{
    [Fact]
    public async Task EnrollStudent_ValidInput_EnrollsStudent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var student = CreateTestStudent(userId: userId);
        var userDTO = CreateTestUserDTO(userId);
        var courseId = Guid.NewGuid();
        var courseDTO = CreateTestCourseDTO(courseId);

        MockGetStudentByUserIdAsync(userId, student);
        MockGetUserByIdAsync(userId, userDTO);
        MockGetCourseByIdAsync(courseId, courseDTO);
        MockUpdateStudentAsync();

        // Act
        await _studentService.EnrollStudentAsync(userId, courseId);

        // Assert
        _studentRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Student>(
            s => s.Id == student.Id && s.Enrollments.Any(e => e.CourseId == courseId))),
            Times.Once());
    }

    [Fact]
    public async Task EnrollStudent_NonExistingStudent_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        _studentRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync((Student)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _studentService.EnrollStudentAsync(userId, courseId));
    }

    [Fact]
    public async Task EnrollStudent_NonExistingCourse_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var student = CreateTestStudent(userId: userId);
        var userDTO = CreateTestUserDTO(userId);
        var courseId = Guid.NewGuid();

        MockGetStudentByUserIdAsync(userId, student);
        MockGetUserByIdAsync(userId, userDTO);
        _courseServiceClientMock.Setup(client => client.GetCourseByIdAsync(courseId)).ReturnsAsync((CourseDTO)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _studentService.EnrollStudentAsync(userId, courseId));
    }
}