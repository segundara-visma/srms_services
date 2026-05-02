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
        var userId = Guid.NewGuid();
        var admins = new List<AdminDTO>
        {
            CreateTestAdminDTO(
                userId,
                new ProfileDTO(
                    "123 Main St",
                    null, null, null, null,
                    null, null, null,
                    null, null, null, null, null
                )
            )
        };
        MockGetUsersByRoleAsync(role, admins, page: 1, pageSize: 10, totalCount: 1);

        var result = await _adminService.GetAllUsersByRoleAsync(role);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        var user = result.Items.Single();
        user.Email.Should().Be("john.doe@example.com");
        user.Profile.Should().NotBeNull();
        user.Profile.Address.Should().Be("123 Main St");
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(1);
        result.TotalPages.Should().Be(1);
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
        MockGetUsersByRoleAsync(role, new List<AdminDTO>(), page: 1, pageSize: 10, totalCount: 0);

        var result = await _adminService.GetAllUsersByRoleAsync(role);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }
}