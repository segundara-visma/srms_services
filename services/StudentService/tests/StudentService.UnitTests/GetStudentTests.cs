using StudentService.Application.DTOs;
using Xunit;
using System;
using System.Threading.Tasks;

namespace StudentService.UnitTests;

public class GetStudentTests : BaseTest
{
    [Fact]
    public async Task GetStudentByIdAsync_ValidUserId_ReturnsStudentDTO()
    {
        var userId = Guid.NewGuid();
        var student = CreateTestStudent(userId: userId);

        var profile = new ProfileDTO(
            "123 Main St", "555-0123",
            null, null, null, null, null,
            null, null, null, null, null, null
        );

        var userDTO = CreateTestUserDTO(userId, profile);

        MockGetStudentByUserIdAsync(userId, student);
        MockGetUserByIdAsync(userId, userDTO);

        var result = await _studentService.GetStudentByIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(student.Id, result.Id);
        Assert.Equal(profile.Address, result.Profile?.Address);
    }

    [Fact]
    public async Task GetStudentByIdAsync_NonExistingStudent_ThrowsException()
    {
        var userId = Guid.NewGuid();
        MockGetStudentByUserIdAsync(userId, null);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _studentService.GetStudentByIdAsync(userId));
    }
}