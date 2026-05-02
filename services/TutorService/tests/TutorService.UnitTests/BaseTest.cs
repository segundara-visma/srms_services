using Moq;
using Moq.Language.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using TutorService.Application.Common;
using TutorService.Application.DTOs;
using TutorService.Application.Interfaces;
using TutorService.Application.Services;
using TutorService.Domain.Entities;

namespace TutorService.UnitTests;

public abstract class BaseTest
{
    protected readonly Mock<ITutorRepository> TutorRepositoryMock;
    protected readonly Mock<IUserServiceClient> UserServiceClientMock;
    protected readonly Mock<IGradeServiceClient> GradeServiceClientMock;

    protected BaseTest()
    {
        TutorRepositoryMock = new Mock<ITutorRepository>();
        UserServiceClientMock = new Mock<IUserServiceClient>();
        GradeServiceClientMock = new Mock<IGradeServiceClient>();
    }

    protected Tutor CreateTestTutor(Guid? tutorId = null, Guid? userId = null)
    {
        return new Tutor
        {
            Id = tutorId ?? Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid()
        };
    }

    protected UserDTO CreateTestUserDTO(Guid userId, ProfileDTO? profile = null)
    {
        return new UserDTO(
            userId,
            "Tutor",
            "One",
            "tutor1@example.com",
            "Tutor",
            profile
        );
    }

    protected UpdateRequestDTO CreateTestUpdateRequest(
        Guid id,
        string? address = null,
        string? phone = null)
    {
        return new UpdateRequestDTO(
            id,
            "Tutor",
            "Two",
            "tutor2@example.com",
            address,
            phone,
            null, // City
            null, // State
            null, // ZipCode
            null, // Country
            null, // Nationality
            null, // Bio
            null, // Facebook
            null, // Twitter
            null, // LinkedIn
            null, // Instagram
            null  // Website
        );
    }

    protected void MockGetUserByIdAsync(Guid userId, UserDTO userDTO)
    {
        UserServiceClientMock
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(userDTO);
    }

    protected void MockGetTutorByUserIdAsync(Guid userId, Tutor? tutor)
    {
        TutorRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(tutor);
    }

    protected void MockUpdateUserAsync(Guid userId, UpdateRequestDTO request, UserDTO? userDTO)
    {
        UserServiceClientMock
            .Setup(x => x.UpdateUserAsync(userId, request))
            .ReturnsAsync(userDTO);
    }

    protected void MockGetUsersByRoleAsync(string role, IEnumerable<UserDTO> users)
    {
        var list = users?.ToList() ?? new List<UserDTO>();

        var response = new PaginatedResponse<UserDTO>
        {
            Items = list,
            TotalCount = list.Count,
            Page = 1,
            PageSize = list.Count
        };

        UserServiceClientMock
            .Setup(x => x.GetUsersByRoleAsync(role, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(response);
    }
}