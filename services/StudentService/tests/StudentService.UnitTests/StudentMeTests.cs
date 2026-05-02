using StudentService.Application.DTOs;
using Xunit;
using System;
using System.Threading.Tasks;

namespace StudentService.UnitTests;

public class StudentMeTests : BaseTest
{
    [Fact]
    public async Task GetMe_ValidUserId_ReturnsStudentDTO()
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
        Assert.Equal(profile.Address, result.Profile?.Address);
    }

    [Fact]
    public async Task UpdateMe_ValidRequest_ReturnsUpdatedStudent()
    {
        var userId = Guid.NewGuid();
        var student = CreateTestStudent(userId: userId);

        var profile = new ProfileDTO(
            "456 Oak St", "555-4567",
            null, null, null, null, null,
            null, null, null, null, null, null
        );

        var request = CreateTestUpdateRequest(userId, profile.Address, profile.Phone);
        var updatedUser = CreateTestUserDTO(userId, profile);

        MockGetStudentByUserIdAsync(userId, student);
        MockUpdateUserAsync(userId, request, updatedUser);

        var result = await _studentService.UpdateStudentAsync(userId, request);

        Assert.NotNull(result);
        Assert.Equal(profile.Address, result.Profile?.Address);
    }
}