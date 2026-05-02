using FluentAssertions;
using Moq;
using StudentService.Application.Interfaces;
using StudentService.Application.Services;
using StudentService.Domain.Entities;
using Xunit;
using System;
using System.Threading.Tasks;

namespace StudentService.UnitTests;

public class CreateStudentTests
{
    private readonly Mock<IStudentRepository> _repo;
    private readonly Mock<IUserServiceClient> _userClient;
    private readonly IStudentService _service;

    public CreateStudentTests()
    {
        _repo = new Mock<IStudentRepository>();
        _userClient = new Mock<IUserServiceClient>();
        _service = new StudentServiceImpl(_repo.Object, _userClient.Object);
    }

    [Fact]
    public async Task CreateStudentAsync_ShouldCreate()
    {
        var userId = Guid.NewGuid();

        _repo.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(() => (Student?)null);

        _repo.Setup(x => x.AddAsync(It.IsAny<Student>()))
            .Returns(Task.CompletedTask);

        await _service.CreateStudentAsync(userId);

        _repo.Verify(x => x.AddAsync(It.IsAny<Student>()), Times.Once);
    }
}