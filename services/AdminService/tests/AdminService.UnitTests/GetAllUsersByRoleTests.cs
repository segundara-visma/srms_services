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
        var role = "Tutor";
        var admins = new List<AdminDTO> { CreateTestAdminDTO(Guid.NewGuid(), new Profile { Address = "123 Main St" }) };
        MockGetUsersByRoleAsync(role, admins);

        var result = await _adminService.GetAllUsersByRoleAsync(role);

        result.Should().HaveCount(1);
        var user = result.Single();
        user.Email.Should().Be("john.doe@example.com");
        user.Profile.Should().NotBeNull();
        user.Profile.Address.Should().Be("123 Main St");
    }

    [Fact]
    public async Task GetAllUsersByRoleAsync_WhenEmptyRole_ThrowsArgumentException()
    {
        var role = "";

        await Assert.ThrowsAsync<ArgumentException>(() => _adminService.GetAllUsersByRoleAsync(role));
    }

    [Fact]
    public async Task GetAllUsersByRoleAsync_WhenNoUsers_ReturnsEmptyList()
    {
        var role = "Tutor";
        MockGetUsersByRoleAsync(role, new List<AdminDTO>());

        var result = await _adminService.GetAllUsersByRoleAsync(role);

        result.Should().BeEmpty();
    }
}