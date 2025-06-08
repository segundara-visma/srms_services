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
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var role = "Tutor";
        var password = "123";
        var userId = Guid.NewGuid();
        var userDTO = new UserDTO(Guid.NewGuid(), firstName, lastName, email, password, role); // Match input parameters
        MockCreateUserAsync(userDTO, userId);
        MockTutorCreateAsync(userId);

        var result = await _adminService.CreateUserAsync(firstName, lastName, email, password, role);

        result.Should().Be(userId);
        UserServiceClientMock.Verify(c => c.CreateUserAsync(It.Is<UserDTO>(u =>
            u.FirstName == firstName &&
            u.LastName == lastName &&
            u.Email == email &&
            u.Password == password &&
            u.Role == role)), Times.Once());
        TutorServiceClientMock.Verify(c => c.CreateTutorAsync(userId), Times.Once());
    }

    [Fact]
    public async Task CreateUserAsync_WhenValidInput_CreatesStudent()
    {
        var firstName = "Jane";
        var lastName = "Doe";
        var email = "jane.doe@example.com";
        var role = "Student";
        var password = "123";
        var userId = Guid.NewGuid();
        var userDTO = new UserDTO(Guid.NewGuid(), firstName, lastName, email, password, role); // Match input parameters
        MockCreateUserAsync(userDTO, userId);
        MockStudentCreateAsync(userId);

        var result = await _adminService.CreateUserAsync(firstName, lastName, email, password, role);

        result.Should().Be(userId);
        UserServiceClientMock.Verify(c => c.CreateUserAsync(It.Is<UserDTO>(u =>
            u.FirstName == firstName &&
            u.LastName == lastName &&
            u.Email == email &&
            u.Password == password &&
            u.Role == role)), Times.Once());
        StudentServiceClientMock.Verify(c => c.CreateStudentAsync(userId), Times.Once());
    }

    [Fact]
    public async Task CreateUserAsync_WhenEmptyFirstName_ThrowsArgumentException()
    {
        var firstName = "";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var role = "Tutor";
        var password = "123";

        await Assert.ThrowsAsync<ArgumentException>(() => _adminService.CreateUserAsync(firstName, lastName, email, password, role));
    }

    [Fact]
    public async Task CreateUserAsync_WhenEmptyRole_ThrowsArgumentException()
    {
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var role = "";
        var password = "123";

        await Assert.ThrowsAsync<ArgumentException>(() => _adminService.CreateUserAsync(firstName, lastName, email, password, role));
    }
}