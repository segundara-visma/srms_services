using Moq;
using StudentService.Application.DTOs;
using StudentService.Domain.Entities;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentService.UnitTests;

public class GetAllStudentsTests : BaseTest
{
    [Fact]
    public async Task GetAllStudentsAsync_ReturnsStudents()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var student1 = CreateTestStudent(userId: userId1);
        var student2 = CreateTestStudent(userId: userId2);

        var profile = new ProfileDTO(
            "123 Main St",
            "555-0123",
            null, null, null, null, null,
            null, null, null, null, null, null
        );

        var users = new List<UserDTO>
        {
            CreateTestUserDTO(userId1, profile),
            CreateTestUserDTO(userId2)
        };

        MockGetUsersByRoleAsync("Student", users);

        MockGetStudentByUserIdAsync(userId1, student1);
        MockGetStudentByUserIdAsync(userId2, student2);

        // IMPORTANT: repository bulk call must also be mocked
        _studentRepositoryMock
            .Setup(x => x.GetByUserIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<Student> { student1, student2 });

        // Act
        var result = await _studentService.GetAllStudentsAsync();

        var list = result.Items.ToList();

        // Assert
        Assert.Equal(2, list.Count);
        Assert.Contains(list, x => x.UserId == userId1);
        Assert.Contains(list, x => x.UserId == userId2);
    }
}