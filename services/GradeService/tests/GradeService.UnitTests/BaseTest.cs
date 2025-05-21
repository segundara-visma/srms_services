using Moq;
using GradeService.Application.Interfaces;
using GradeService.Application.Services;
using Xunit;

namespace GradeService.UnitTests;

public class BaseTest : IDisposable
{
    protected readonly Mock<IGradeRepository> GradeRepositoryMock;
    protected readonly Mock<IEnrollmentServiceClient> EnrollmentServiceClientMock;
    protected readonly GradeServiceImpl GradeService;

    public BaseTest()
    {
        GradeRepositoryMock = new Mock<IGradeRepository>();
        EnrollmentServiceClientMock = new Mock<IEnrollmentServiceClient>();
        GradeService = new GradeServiceImpl(GradeRepositoryMock.Object, EnrollmentServiceClientMock.Object);
    }

    public void Dispose() // ensures mocks are reset, preventing side effects between tests
    {
        // Cleanup if needed (e.g., reset mocks)
        GradeRepositoryMock.Reset();
        EnrollmentServiceClientMock.Reset();
    }
}