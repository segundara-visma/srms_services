using FluentAssertions;
using Moq;
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
        var profile = new Profile { Address = "456 Oak St", Phone = "555-1234" };
        var updateRequest = CreateTestUpdateRequest(userId, profile.Address, profile.Phone);
        var updatedAdminDTO = CreateTestAdminDTO(userId, profile) with { FirstName = updateRequest.FirstName, LastName = updateRequest.LastName, Email = updateRequest.Email };
        MockUpdateUserAsync(userId, updateRequest, updatedAdminDTO);

        var result = await _adminService.UpdateAdminAsync(userId, updateRequest);

        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.FirstName.Should().Be(updateRequest.FirstName); // "Jane"
        result.LastName.Should().Be(updateRequest.LastName);  // "Doe"
        result.Email.Should().Be(updateRequest.Email);       // "jane.doe@example.com"
        result.Profile.Should().NotBeNull();
        result.Profile.Address.Should().Be(profile.Address);
        result.Profile.Phone.Should().Be(profile.Phone);
    }

    [Fact]
    public async Task UpdateAdminAsync_WhenUpdateFails_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var updateRequest = CreateTestUpdateRequest(userId);
        MockUpdateUserAsync(userId, updateRequest, null);

        await Assert.ThrowsAsync<ArgumentException>(() => _adminService.UpdateAdminAsync(userId, updateRequest));
    }
}