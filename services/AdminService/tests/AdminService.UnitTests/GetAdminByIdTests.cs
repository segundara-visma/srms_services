using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using AdminService.Application.Interfaces;
using AdminService.Application.Services;
using AdminService.Application.DTOs;
using Xunit;

namespace AdminService.UnitTests;

public class GetAdminByIdTests : BaseTest
{
    private readonly IAdminService _adminService;

    public GetAdminByIdTests()
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
    public async Task GetAdminByIdAsync_WhenAdminExists_ReturnsAdminDTO()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile { Address = "123 Main St" };
        var adminDTO = CreateTestAdminDTO(userId, profile);
        MockGetUserByIdAsync(userId, adminDTO);

        var result = await _adminService.GetAdminByIdAsync(userId);

        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Email.Should().Be(adminDTO.Email);
        result.Profile.Should().NotBeNull();
        result.Profile.Address.Should().Be(profile.Address);
    }

    [Fact]
    public async Task GetAdminByIdAsync_WhenUserNotFound_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        MockGetUserByIdAsync(userId, null);

        await Assert.ThrowsAsync<ArgumentException>(() => _adminService.GetAdminByIdAsync(userId));
    }

    [Fact]
    public async Task GetAdminByIdAsync_WhenNonAdminRole_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var adminDTO = CreateTestAdminDTO(userId, null); // Default role is "Admin", change to non-admin
        adminDTO = adminDTO with { Role = "Student" }; // Use with-expression to modify Role
        MockGetUserByIdAsync(userId, adminDTO);

        await Assert.ThrowsAsync<ArgumentException>(() => _adminService.GetAdminByIdAsync(userId));
    }
}