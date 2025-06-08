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
        var profile = new Profile { Address = "123 Main St", Phone = "555-0123" };
        var userDTO = CreateTestUserDTO(userId, profile);

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
        Assert.Equal(userDTO.Role, result.Role);
        Assert.NotNull(result.Profile);
        Assert.Equal(profile.Address, result.Profile.Address);
        Assert.Equal(profile.Phone, result.Profile.Phone);
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
        var profile = new Profile { };
        var student = CreateTestStudent(userId: userId);
        var userDTO = new UserDTO(userId, "John", "Doe", "john.doe@example.com", "Teacher", profile); // Wrong role

        MockGetStudentByUserIdAsync(userId, student);
        MockGetUserByIdAsync(userId, userDTO);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _studentService.GetStudentByIdAsync(userId));
        Assert.Contains($"User with ID {userId} is not a student", exception.Message);
    }
}