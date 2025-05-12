using StudentService.Application.DTOs;
using StudentService.Domain.Entities;
using Moq;
using Xunit;
using System.Threading.Tasks;

namespace StudentService.UnitTests;

public class GetStudentTests : BaseTest
{
    [Fact]
    public async Task GetStudentById_ExistingStudent_ReturnsStudentDTO()
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
    public async Task GetStudentById_NonExistingStudent_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _studentRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync((Student)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _studentService.GetStudentByIdAsync(userId));
    }
}