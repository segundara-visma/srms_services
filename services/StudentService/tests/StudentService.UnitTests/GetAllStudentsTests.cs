using Moq;
using StudentService.Domain.Entities;
using StudentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace StudentService.UnitTests;

public class GetAllStudentsTests : BaseTest
{
    [Fact]
    public async Task GetAllStudentsAsync_StudentsExist_ReturnsStudentDTOs()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var student1 = CreateTestStudent(userId: userId1);
        var student2 = CreateTestStudent(userId: userId2);
        var userDTO1 = CreateTestUserDTO(userId1);
        var userDTO2 = CreateTestUserDTO(userId2);

        var users = new List<UserDTO> { userDTO1, userDTO2 };
        MockGetUsersByRoleAsync("Student", users);
        MockGetStudentByUserIdAsync(userId1, student1);
        MockGetStudentByUserIdAsync(userId2, student2);

        // Act
        var result = await _studentService.GetAllStudentsAsync();

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Contains(resultList, s => s.UserId == userId1 && s.Id == student1.Id);
        Assert.Contains(resultList, s => s.UserId == userId2 && s.Id == student2.Id);
    }

    [Fact]
    public async Task GetAllStudentsAsync_NoStudents_ReturnsEmptyList()
    {
        // Arrange
        MockGetUsersByRoleAsync("Student", new List<UserDTO>());

        // Act
        var result = await _studentService.GetAllStudentsAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllStudentsAsync_SomeStudentsNotFound_ReturnsOnlyFoundStudents()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var student1 = CreateTestStudent(userId: userId1);
        var userDTO1 = CreateTestUserDTO(userId1);
        var userDTO2 = new UserDTO(userId2, "Jane", "Doe", "jane.doe@example.com", "Student");

        var users = new List<UserDTO> { userDTO1, userDTO2 };
        MockGetUsersByRoleAsync("Student", users);
        MockGetStudentByUserIdAsync(userId1, student1);
        MockGetStudentByUserIdAsync(userId2, null);

        // Act
        var result = await _studentService.GetAllStudentsAsync();

        // Assert
        var resultList = result.ToList();
        Assert.Single(resultList);
        Assert.Contains(resultList, s => s.UserId == userId1 && s.Id == student1.Id);
    }
}