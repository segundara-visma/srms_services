using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdminService.Application.Interfaces;
using AdminService.Application.Services;
using AdminService.Application.DTOs;
using Xunit;

namespace AdminService.UnitTests;

public class GetSystemOverviewTests : BaseTest
{
    private readonly IAdminService _adminService;

    public GetSystemOverviewTests()
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
    public async Task GetSystemOverviewAsync_WhenDataExists_ReturnsOverview()
    {
        var tutors = new List<AdminDTO> { CreateTestAdminDTO(Guid.NewGuid(), null) };
        var students = new List<AdminDTO> { CreateTestAdminDTO(Guid.NewGuid(), null) };

        var grades = new List<GradeDTO>
        {
            new GradeDTO(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                85m,
                DateTime.UtcNow,
                "Good"
            )
        };

        var courses = new List<object> { new { } };
        var enrollments = new List<object> { new { } };

        MockGetAllUsersByRoleAsync("Tutor", tutors);
        MockGetAllUsersByRoleAsync("Student", students);
        MockGetAllGradesAsync(grades);
        MockGetAllCoursesAsync(courses);
        MockGetAllEnrollmentsAsync(enrollments);

        var result = await _adminService.GetSystemOverviewAsync();

        result.Should().BeEquivalentTo(new SystemOverviewDTO(
            1, 1, 1, 1, 1, 85m));
    }

    [Fact]
    public async Task GetSystemOverviewAsync_WhenNoData_ReturnsZeroValues()
    {
        MockGetAllUsersByRoleAsync("Tutor", new List<AdminDTO>());
        MockGetAllUsersByRoleAsync("Student", new List<AdminDTO>());
        MockGetAllGradesAsync(new List<GradeDTO>());
        MockGetAllCoursesAsync(new List<object>());
        MockGetAllEnrollmentsAsync(new List<object>());

        var result = await _adminService.GetSystemOverviewAsync();

        result.Should().BeEquivalentTo(new SystemOverviewDTO(
            0, 0, 0, 0, 0, 0m));
    }
}