using EnrollmentService.Domain.Entities;
using EnrollmentService.Application.DTOs;
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;

namespace EnrollmentService.UnitTests;

public class CancelEnrollmentTests : BaseTest
{
    [Fact]
    public async Task CancelEnrollmentAsync_ValidId_UpdatesStatus()
    {
        // Arrange
        var enrollmentId = Guid.NewGuid();
        var enrollment = CreateSampleEnrollment(Guid.NewGuid(), Guid.NewGuid());
        enrollment.Id = enrollmentId;
        MockRepository.Setup(x => x.GetByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        MockRepository.Setup(x => x.UpdateAsync(It.IsAny<Enrollment>()))
            .Returns(Task.CompletedTask);

        // Act
        var exception = await Record.ExceptionAsync(() => Service.CancelEnrollmentAsync(enrollmentId));

        // Assert
        Assert.Null(exception);
        MockRepository.Verify(x => x.GetByIdAsync(enrollmentId), Times.Once());
        MockRepository.Verify(x => x.UpdateAsync(It.Is<Enrollment>(e => e.Status == EnrollmentStatus.Cancelled)), Times.Once());
    }
}