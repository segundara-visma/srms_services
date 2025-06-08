using StudentService.Application.DTOs;
using StudentService.Domain.Entities;
using Moq;
using Xunit;
using System.Threading.Tasks;

namespace StudentService.UnitTests;

public class StudentMeTests : BaseTest
{
    [Fact]
    public async Task GetMe_ValidUserId_ReturnsStudentDTO()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var student = CreateTestStudent(userId: userId);
        var profile = new Profile { Address = "123 Main St", Phone = "555-0123" };
        var userDTO = CreateTestUserDTO(userId, profile);

        MockGetStudentByUserIdAsync(userId, student);
        MockGetUserByIdAsync(userId, userDTO);

        // Act
        var result = await _studentService.GetStudentByIdAsync(userId); // Assuming GetMe uses this internally

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
    public async Task GetMe_NonExistingStudent_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        MockGetStudentByUserIdAsync(userId, null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _studentService.GetStudentByIdAsync(userId));
        Assert.Contains($"Student with User ID {userId} not found", exception.Message);
    }

    [Fact]
    public async Task UpdateMe_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var student = CreateTestStudent(userId: userId);
        var profile = new Profile { Address = "456 Oak St", Phone = "555-4567" };
        var updateRequest = CreateTestUpdateRequest(userId, profile.Address, profile.Phone);
        // Use updated values from updateRequest for the returned UserDTO
        var updatedUserDTO = new UserDTO(userId, updateRequest.FirstName, updateRequest.LastName, updateRequest.Email, "Student", profile);

        MockGetStudentByUserIdAsync(userId, student);
        MockUpdateUserAsync(userId, updateRequest, updatedUserDTO);

        // Act
        var result = await _studentService.UpdateStudentAsync(userId, updateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(student.Id, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(updateRequest.FirstName, result.FirstName); // Should now match "Jane"
        Assert.Equal(updateRequest.LastName, result.LastName);  // Should now match "Doe"
        Assert.Equal(updateRequest.Email, result.Email);       // Should now match "jane.doe@example.com"
        Assert.Equal("Student", result.Role); // Assuming role remains unchanged
        Assert.NotNull(result.Profile);
        Assert.Equal(profile.Address, result.Profile.Address);
        Assert.Equal(profile.Phone, result.Profile.Phone);
    }

    [Fact]
    public async Task UpdateMe_NonExistingStudent_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateRequest = CreateTestUpdateRequest(userId);

        MockGetStudentByUserIdAsync(userId, null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _studentService.UpdateStudentAsync(userId, updateRequest));
        Assert.Contains($"Student with User ID {userId} not found", exception.Message);
    }

    [Fact]
    public async Task UpdateMe_FailedUpdate_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var student = CreateTestStudent(userId: userId);
        var updateRequest = CreateTestUpdateRequest(userId);

        MockGetStudentByUserIdAsync(userId, student);
        MockUpdateUserAsync(userId, updateRequest, null); // Simulate failed update

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _studentService.UpdateStudentAsync(userId, updateRequest));
        Assert.Contains("Update request failed", exception.Message);
    }
}