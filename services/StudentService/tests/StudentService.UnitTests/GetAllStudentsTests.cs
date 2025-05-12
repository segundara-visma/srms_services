using StudentService.Application.DTOs;
using StudentService.Domain.Entities;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentService.UnitTests;

public class GetAllStudentsTests : BaseTest
{
    [Fact]
    public async Task GetAllStudents_MultipleStudents_ReturnsAllStudents()
    {
        // Arrange
        var user1 = CreateTestUserDTO(Guid.NewGuid());
        var user2 = CreateTestUserDTO(Guid.NewGuid());
        var users = new List<UserDTO> { user1, user2 };
        MockGetUsersByRoleAsync("Student", users);

        var student1 = CreateTestStudent(userId: user1.Id);
        var student2 = CreateTestStudent(userId: user2.Id);
        MockGetStudentByUserIdAsync(user1.Id, student1);
        MockGetStudentByUserIdAsync(user2.Id, student2);

        // Act
        var result = await _studentService.GetAllStudentsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, s => s.UserId == user1.Id && s.FirstName == user1.FirstName);
        Assert.Contains(result, s => s.UserId == user2.Id && s.FirstName == user2.FirstName);
    }

    [Fact]
    public async Task GetAllStudents_NoStudents_ReturnsEmptyList()
    {
        // Arrange
        MockGetUsersByRoleAsync("Student", new List<UserDTO>());

        // Act
        var result = await _studentService.GetAllStudentsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}