using Moq;
using StudentService.Application.Common;
using StudentService.Application.DTOs;
using StudentService.Application.Interfaces;
using StudentService.Application.Services;
using StudentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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

        _studentService = new StudentServiceImpl(
            _studentRepositoryMock.Object,
            _userServiceClientMock.Object);
    }

    protected Student CreateTestStudent(Guid? studentId = null, Guid? userId = null)
    {
        return new Student(userId ?? Guid.NewGuid())
        {
            Id = studentId ?? Guid.NewGuid()
        };
    }

    protected UserDTO CreateTestUserDTO(Guid userId, ProfileDTO? profile = null)
    {
        return new UserDTO(
            userId,
            "John",
            "Doe",
            "john.doe@example.com",
            "Student",
            profile
        );
    }

    protected UpdateRequestDTO CreateTestUpdateRequest(Guid id, string? address = null, string? phone = null)
    {
        return new UpdateRequestDTO(
            id,
            "Jane",
            "Doe",
            "jane.doe@example.com",
            address,
            phone,
            null, null, null, null, null, null, null, null, null, null, null
        );
    }

    protected void MockGetUserByIdAsync(Guid userId, UserDTO? userDTO)
    {
        _userServiceClientMock
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(userDTO);
    }

    // FIXED (this was missing)
    protected void MockGetStudentByUserIdAsync(Guid userId, Student? student)
    {
        _studentRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(student);
    }

    protected void MockGetUsersByRoleAsync(string role, IEnumerable<UserDTO> users)
    {
        var list = users?.ToList() ?? new List<UserDTO>();

        var response = new PaginatedResponse<UserDTO>
        {
            Items = list,
            Page = 1,
            PageSize = list.Count,
            TotalCount = list.Count
        };

        _userServiceClientMock
            .Setup(x => x.GetUsersByRoleAsync(role, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(response);
    }

    protected void MockUpdateUserAsync(Guid userId, UpdateRequestDTO request, UserDTO? userDTO)
    {
        _userServiceClientMock
            .Setup(x => x.UpdateUserAsync(userId, request))
            .ReturnsAsync(userDTO);
    }
}