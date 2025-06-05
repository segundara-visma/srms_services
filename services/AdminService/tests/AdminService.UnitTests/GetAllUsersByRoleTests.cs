using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdminService.Application.Interfaces;
using AdminService.Application.Services;
using AdminService.Application.DTOs;
using Xunit;

namespace AdminService.UnitTests;

public class GetAllUsersByRoleTests : BaseTest
{
    private readonly IAdminService _adminService;

    public GetAllUsersByRoleTests()
    {
        _adminService = new AdminServiceImpl(
            UserServiceClientMock.Object,
            TutorServiceClientMock.Object,
            StudentServiceClientMock.Object,
            GradeServiceClientMock.Object,
            CourseServiceClientMock.Object,
            EnrollmentServiceClientMock.Object);
    }

    [Fact]
    public async Task GetAllUsersByRoleAsync_WhenRoleExists_ReturnsUsers()
    {
        // Arrange
        var role = "Tutor";
        var users = new List<UserDTO>
        {
            new UserDTO(Guid.NewGuid(), "Tutor", "One", "tutor1@example.com", "123", "Tutor")
        };
        UserServiceClientMock.Setup(c => c.GetUsersByRoleAsync(role)).ReturnsAsync(users);

        // Act
        var result = await _adminService.GetAllUsersByRoleAsync(role);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(u => u.Email == "tutor1@example.com");
    }
}