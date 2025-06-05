using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using AdminService.Application.Interfaces;
using AdminService.Application.Services;
using AdminService.Application.DTOs;
using Xunit;

namespace AdminService.UnitTests;

public class CreateUserTests : BaseTest
{
    private readonly IAdminService _adminService;

    public CreateUserTests()
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
    public async Task CreateUserAsync_WhenValidInput_CreatesTutor()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var role = "Tutor";
        var password = "123";
        var userId = Guid.NewGuid();
        UserServiceClientMock.Setup(c => c.CreateUserAsync(It.IsAny<UserDTO>())).ReturnsAsync(userId);
        TutorServiceClientMock.Setup(c => c.CreateTutorAsync(userId)).Returns(Task.CompletedTask);

        // Act
        var result = await _adminService.CreateUserAsync(firstName, lastName, email, password, role);

        // Assert
        result.Should().Be(userId);
        UserServiceClientMock.Verify(c => c.CreateUserAsync(It.Is<UserDTO>(u => u.Email == email && u.Role == role)), Times.Once());
        TutorServiceClientMock.Verify(c => c.CreateTutorAsync(userId), Times.Once());
    }

    [Fact]
    public async Task CreateUserAsync_WhenValidInput_CreatesStudent()
    {
        // Arrange
        var firstName = "Jane";
        var lastName = "Doe";
        var email = "jane.doe@example.com";
        var role = "Student";
        var password = "123";
        var userId = Guid.NewGuid();
        UserServiceClientMock.Setup(c => c.CreateUserAsync(It.IsAny<UserDTO>())).ReturnsAsync(userId);
        StudentServiceClientMock.Setup(c => c.CreateStudentAsync(userId)).Returns(Task.CompletedTask);

        // Act
        var result = await _adminService.CreateUserAsync(firstName, lastName, email, password, role);

        // Assert
        result.Should().Be(userId);
        UserServiceClientMock.Verify(c => c.CreateUserAsync(It.Is<UserDTO>(u => u.Email == email && u.Role == role)), Times.Once());
        StudentServiceClientMock.Verify(c => c.CreateStudentAsync(userId), Times.Once());
    }
}