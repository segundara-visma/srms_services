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
    private readonly IStudentService _studentService;

    public CreateStudentTests()
    {
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _studentService = new StudentServiceImpl(_studentRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateStudentAsync_WhenValidInput_CreatesStudent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _studentRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Student)null);
        _studentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Student>())).Returns(Task.CompletedTask);

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
        var existingStudent = new Student { Id = Guid.NewGuid(), UserId = userId };
        _studentRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existingStudent);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _studentService.CreateStudentAsync(userId));
    }
}