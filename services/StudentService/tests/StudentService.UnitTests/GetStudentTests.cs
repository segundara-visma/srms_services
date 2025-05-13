using StudentService.Application.DTOs;
using StudentService.Domain.Entities;
using Moq;
using Xunit;
using System.Threading.Tasks;

namespace StudentService.UnitTests;

public class GetStudentTests : BaseTest
{
    [Fact]
    public async Task GetStudentByIdAsync_ValidUserId_ReturnsStudentDTO()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var student = CreateTestStudent(userId: userId);
        var userDTO = CreateTestUserDTO(userId);

        MockGetStudentByUserIdAsync(userId, student);
        MockGetUserByIdAsync(userId, userDTO);

        // Act
        var result = await _studentService.GetStudentByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(student.Id, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(userDTO.FirstName, result.FirstName);
        Assert.Equal(userDTO.LastName, result.LastName);
        Assert.Equal(userDTO.Email, result.Email);
    }

    [Fact]
    public async Task GetStudentByIdAsync_NonExistingStudent_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        MockGetStudentByUserIdAsync(userId, null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _studentService.GetStudentByIdAsync(userId));
        Assert.Contains($"Student with User ID {userId} not found", exception.Message);
    }

    [Fact]
    public async Task GetStudentByIdAsync_NonStudentUser_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var student = CreateTestStudent(userId: userId);
        var userDTO = new UserDTO(userId, "John", "Doe", "john.doe@example.com", "Teacher"); // Wrong role

        MockGetStudentByUserIdAsync(userId, student);
        MockGetUserByIdAsync(userId, userDTO);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _studentService.GetStudentByIdAsync(userId));
        Assert.Contains($"User with ID {userId} is not a student", exception.Message);
    }
}