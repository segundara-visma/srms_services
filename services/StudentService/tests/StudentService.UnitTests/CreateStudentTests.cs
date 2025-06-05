using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using StudentService.Application.Services;
using StudentService.Application.Interfaces;
using StudentService.Domain.Entities;
using Xunit;

namespace StudentService.UnitTests;

public class CreateStudentTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<IUserServiceClient> _userServiceClientMock;
    private readonly IStudentService _studentService;

    public CreateStudentTests()
    {
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _userServiceClientMock = new Mock<IUserServiceClient>(); // Mock IUserServiceClient
        _studentService = new StudentServiceImpl(_studentRepositoryMock.Object, _userServiceClientMock.Object);
    }

    [Fact]
    public async Task CreateStudentAsync_WhenValidInput_CreatesStudent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _studentRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Student)null);

        // Use a lambda to create a Student with the correct userId
        _studentRepositoryMock.Setup(r => r.AddAsync(It.Is<Student>(s => s.UserId == userId)))
            .Returns(Task.CompletedTask)
            .Callback<Student>(s => s.Id = Guid.NewGuid()); // Simulate Id assignment if needed

        // Act
        await _studentService.CreateStudentAsync(userId);

        // Assert
        _studentRepositoryMock.Verify(r => r.AddAsync(It.Is<Student>(s => s.UserId == userId)), Times.Once());
    }

    [Fact]
    public async Task CreateStudentAsync_WhenStudentExists_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingStudent = new Student(userId) { Id = Guid.NewGuid() }; // Create with userId
        _studentRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existingStudent);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _studentService.CreateStudentAsync(userId));
    }
}