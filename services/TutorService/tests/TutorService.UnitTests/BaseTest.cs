using Moq;
using System;
using TutorService.Application.Interfaces;

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
    }
}