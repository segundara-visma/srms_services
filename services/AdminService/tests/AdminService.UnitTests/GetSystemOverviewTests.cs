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
        // Arrange
        var tutors = new List<TutorDTO> { new TutorDTO(Guid.NewGuid(), Guid.NewGuid(), "Tutor", "One", "tutor1@example.com") };
        var students = new List<UserDTO> { new UserDTO(Guid.NewGuid(), "Student", "One", "student1@example.com", "123", "Student") };
        var grades = new List<GradeDTO> { new GradeDTO { GradeValue = 85m } };
        var courses = new List<object> { new { } };
        var enrollments = new List<object> { new { } };
        TutorServiceClientMock.Setup(c => c.GetAllTutorsAsync()).ReturnsAsync(tutors);
        UserServiceClientMock.Setup(c => c.GetUsersByRoleAsync("Student")).ReturnsAsync(students);
        GradeServiceClientMock.Setup(c => c.GetAllGradesAsync()).ReturnsAsync(grades);
        CourseServiceClientMock.Setup(c => c.GetAllCoursesAsync()).ReturnsAsync(courses);
        EnrollmentServiceClientMock.Setup(c => c.GetAllEnrollmentsAsync()).ReturnsAsync(enrollments);

        // Act
        var result = await _adminService.GetSystemOverviewAsync();

        // Assert
        result.Should().BeEquivalentTo(new SystemOverviewDTO(
            TotalTutors: 1,
            TotalStudents: 1,
            TotalGrades: 1,
            TotalCourses: 1,
            TotalEnrollments: 1,
            AverageGrade: 85m));
    }
}