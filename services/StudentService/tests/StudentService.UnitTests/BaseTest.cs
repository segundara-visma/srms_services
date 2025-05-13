using Moq;
using StudentService.Application.DTOs;
using StudentService.Application.Interfaces;
using StudentService.Application.Services;
using StudentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentService.UnitTests;

public abstract class BaseTest
{
    protected readonly Mock<IStudentRepository> _studentRepositoryMock;
    protected readonly Mock<IUserServiceClient> _userServiceClientMock;
    protected readonly IStudentService _studentService;

    public BaseTest()
    {
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _userServiceClientMock = new Mock<IUserServiceClient>();
        _studentService = new StudentServiceImpl(_studentRepositoryMock.Object, _userServiceClientMock.Object);
    }

    protected Student CreateTestStudent(Guid? studentId = null, Guid? userId = null)
    {
        return new Student(userId ?? Guid.NewGuid())
        {
            Id = studentId ?? Guid.NewGuid()
        };
    }

    protected UserDTO CreateTestUserDTO(Guid userId)
    {
        return new UserDTO(userId, "John", "Doe", "john.doe@example.com", "Student");
    }

    protected void MockGetUserByIdAsync(Guid userId, UserDTO userDTO)
    {
        _userServiceClientMock.Setup(client => client.GetUserByIdAsync(userId)).ReturnsAsync(userDTO);
    }

    protected void MockGetUsersByRoleAsync(string role, IEnumerable<UserDTO> users)
    {
        _userServiceClientMock.Setup(client => client.GetUsersByRoleAsync(role)).ReturnsAsync(users);
    }

    protected void MockGetStudentByIdAsync(Student student)
    {
        _studentRepositoryMock.Setup(repo => repo.GetByIdAsync(student.Id)).ReturnsAsync(student);
    }

    protected void MockGetStudentByUserIdAsync(Guid userId, Student student)
    {
        _studentRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(student);
    }
}