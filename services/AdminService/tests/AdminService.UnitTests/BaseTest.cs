using Moq;
using AdminService.Application.Interfaces;

namespace AdminService.UnitTests;

public abstract class BaseTest
{
    protected readonly Mock<IUserServiceClient> UserServiceClientMock;
    protected readonly Mock<ITutorServiceClient> TutorServiceClientMock;
    protected readonly Mock<IStudentServiceClient> StudentServiceClientMock;
    protected readonly Mock<IGradeServiceClient> GradeServiceClientMock;
    protected readonly Mock<ICourseServiceClient> CourseServiceClientMock;
    protected readonly Mock<IEnrollmentServiceClient> EnrollmentServiceClientMock;

    protected BaseTest()
    {
        UserServiceClientMock = new Mock<IUserServiceClient>();
        TutorServiceClientMock = new Mock<ITutorServiceClient>();
        StudentServiceClientMock = new Mock<IStudentServiceClient>();
        GradeServiceClientMock = new Mock<IGradeServiceClient>();
        CourseServiceClientMock = new Mock<ICourseServiceClient>();
        EnrollmentServiceClientMock = new Mock<IEnrollmentServiceClient>();
    }
}