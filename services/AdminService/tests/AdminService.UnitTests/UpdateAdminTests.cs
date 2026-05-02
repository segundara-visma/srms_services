using FluentAssertions;
using System;
using System.Threading.Tasks;
using AdminService.Application.Interfaces;
using AdminService.Application.Services;
using AdminService.Application.DTOs;
using Xunit;

namespace AdminService.UnitTests;

public class UpdateAdminTests : BaseTest
{
    private readonly IAdminService _adminService;

    public UpdateAdminTests()
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
    public async Task UpdateAdminAsync_WhenValidRequest_ReturnsUpdatedAdminDTO()
    {
        var userId = Guid.NewGuid();

        var profile = new ProfileDTO(
            "456 Oak St", "555-1234", null, null, null, null, null, null,
            null, null, null, null, null
        );

        var updateRequest = CreateTestUpdateRequest(userId, profile.Address, profile.Phone);

        var updatedAdmin = CreateTestAdminDTO(userId, profile) with
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com"
        };

        MockUpdateUserAsync(userId, updateRequest, updatedAdmin);

        var result = await _adminService.UpdateAdminAsync(userId, updateRequest);

        result.Should().NotBeNull();
        result!.Profile!.Address.Should().Be("456 Oak St");
    }

    [Fact]
    public async Task UpdateAdminAsync_WhenUpdateFails_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();

        var updateRequest = CreateTestUpdateRequest(userId);

        MockUpdateUserAsync(userId, updateRequest, null);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _adminService.UpdateAdminAsync(userId, updateRequest));
    }
}