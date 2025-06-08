using FluentAssertions;
using Moq;
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
        var tutors = new List<TutorDTO> { new TutorDTO(Guid.NewGuid(), Guid.NewGuid(), "Tutor", "One", "tutor1@example.com") };
        var students = new List<AdminDTO> { CreateTestAdminDTO(Guid.NewGuid(), null) }; // Use AdminDTO as per interface
        var grades = new List<GradeDTO> { new GradeDTO { GradeValue = 85m } };
        var courses = new List<object> { new { } };
        var enrollments = new List<object> { new { } };
        MockGetAllTutorsAsync(tutors);
        MockGetUsersByRoleAsync("Student", students); // Updated to AdminDTO
        MockGetAllGradesAsync(grades);
        MockGetAllCoursesAsync(courses);
        MockGetAllEnrollmentsAsync(enrollments);

        var result = await _adminService.GetSystemOverviewAsync();

        result.Should().BeEquivalentTo(new SystemOverviewDTO(
            TotalTutors: 1,
            TotalStudents: 1,
            TotalGrades: 1,
            TotalCourses: 1,
            TotalEnrollments: 1,
            AverageGrade: 85m));
    }

    [Fact]
    public async Task GetSystemOverviewAsync_WhenNoData_ReturnsZeroValues()
    {
        MockGetAllTutorsAsync(new List<TutorDTO>());
        MockGetUsersByRoleAsync("Student", new List<AdminDTO>()); // Updated to AdminDTO
        MockGetAllGradesAsync(new List<GradeDTO>());
        MockGetAllCoursesAsync(new List<object>());
        MockGetAllEnrollmentsAsync(new List<object>());

        var result = await _adminService.GetSystemOverviewAsync();

        result.Should().BeEquivalentTo(new SystemOverviewDTO(
            TotalTutors: 0,
            TotalStudents: 0,
            TotalGrades: 0,
            TotalCourses: 0,
            TotalEnrollments: 0,
            AverageGrade: 0m));
    }
}