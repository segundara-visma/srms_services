using Moq;
using ReportService.Application.Interfaces;
using ReportService.Application.Services;
using Xunit;

namespace ReportService.UnitTests;

public class BaseTest : IDisposable, IAsyncDisposable
{
    protected readonly Mock<IReportRepository> ReportRepositoryMock;
    protected readonly Mock<IGradeServiceClient> GradeServiceClientMock;
    protected readonly Mock<IEnrollmentServiceClient> EnrollmentServiceClientMock;
    protected readonly Mock<ICourseServiceClient> CourseServiceClientMock;
    protected readonly ReportServiceImpl ReportService;

    public BaseTest()
    {
        ReportRepositoryMock = new Mock<IReportRepository>();
        GradeServiceClientMock = new Mock<IGradeServiceClient>();
        EnrollmentServiceClientMock = new Mock<IEnrollmentServiceClient>();
        CourseServiceClientMock = new Mock<ICourseServiceClient>();
        ReportService = new ReportServiceImpl(
            ReportRepositoryMock.Object,
            GradeServiceClientMock.Object,
            EnrollmentServiceClientMock.Object,
            CourseServiceClientMock.Object);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            ReportRepositoryMock.Reset();
            GradeServiceClientMock.Reset();
            EnrollmentServiceClientMock.Reset();
            CourseServiceClientMock.Reset();
        }
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}