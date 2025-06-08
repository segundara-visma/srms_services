using Moq;
using System;
using TutorService.Application.DTOs;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;

namespace TutorService.UnitTests
{
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
            return new Tutor { Id = tutorId ?? Guid.NewGuid(), UserId = userId ?? Guid.NewGuid() };
        }

        protected UserDTO CreateTestUserDTO(Guid userId, Profile? profile = null)
        {
            return new UserDTO(userId, "Tutor", "One", "tutor1@example.com", "Tutor", profile);
        }

        protected UpdateRequest CreateTestUpdateRequest(Guid id, string? address = null, string? phone = null)
        {
            return new UpdateRequest(id, "Tutor", "Two", "tutor2@example.com",
                address, phone, null, null, null, null, null, null, null, null, null, null, null);
        }

        protected void MockGetUserByIdAsync(Guid userId, UserDTO userDTO)
        {
            UserServiceClientMock.Setup(client => client.GetUserByIdAsync(userId)).ReturnsAsync(userDTO);
        }

        protected void MockGetTutorByUserIdAsync(Guid userId, Tutor tutor)
        {
            TutorRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(tutor);
        }

        protected void MockUpdateUserAsync(Guid userId, UpdateRequest request, UserDTO userDTO)
        {
            UserServiceClientMock.Setup(client => client.UpdateUserAsync(userId, request)).ReturnsAsync(userDTO);
        }
    }
}