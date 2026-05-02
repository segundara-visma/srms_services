using FluentAssertions;
using GradeService.Application.DTOs;
using GradeService.Domain.Entities;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GradeService.UnitTests;

public class AddGradeTests : BaseTest
{
    [Fact]
    public async Task AddGradeAsync_WhenStudentIsEnrolledAndGradeIsValid_AddsGradeSuccessfully()
    {
        // Arrange
        var gradeDto = new CreateGradeDTO(
            Guid.NewGuid(),
            Guid.NewGuid(),
            88.0m,
            DateTime.UtcNow,
            "Well done"
        );

        EnrollmentServiceClientMock
            .Setup(x => x.CheckEnrollmentAsync(gradeDto.StudentId, gradeDto.CourseId))
            .ReturnsAsync(true);

        GradeRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Grade>()))
            .Returns(Task.CompletedTask);

        // Act
        await GradeService.AddGradeAsync(gradeDto);

        // Assert
        GradeRepositoryMock.Verify(x => x.AddAsync(It.Is<Grade>(g =>
            g.StudentId == gradeDto.StudentId &&
            g.CourseId == gradeDto.CourseId &&
            g.GradeValue == gradeDto.GradeValue &&
            g.Comments == gradeDto.Comments
        )), Times.Once);
    }

    [Fact]
    public async Task AddGradeAsync_WhenStudentIsNotEnrolled_ThrowsInvalidOperationException()
    {
        // Arrange
        var gradeDto = new CreateGradeDTO(
            Guid.NewGuid(),
            Guid.NewGuid(),
            88.0m,
            DateTime.UtcNow,
            null
        );

        EnrollmentServiceClientMock
            .Setup(x => x.CheckEnrollmentAsync(gradeDto.StudentId, gradeDto.CourseId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            GradeService.AddGradeAsync(gradeDto));

        GradeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Grade>()), Times.Never);
    }

    [Fact]
    public async Task AddGradeAsync_WhenGradeValueIsOutOfRange_ThrowsArgumentException()
    {
        // Arrange
        var gradeDto = new CreateGradeDTO(
            Guid.NewGuid(),
            Guid.NewGuid(),
            101m,
            DateTime.UtcNow,
            null
        );

        EnrollmentServiceClientMock
            .Setup(x => x.CheckEnrollmentAsync(gradeDto.StudentId, gradeDto.CourseId))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            GradeService.AddGradeAsync(gradeDto));

        GradeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Grade>()), Times.Never);
    }

    [Fact]
    public async Task AddGradeAsync_WhenGradeDtoIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        CreateGradeDTO? gradeDto = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            GradeService.AddGradeAsync(gradeDto!));

        GradeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Grade>()), Times.Never);
    }

    [Fact]
    public async Task AddGradeAsync_WhenGradedAtIsDefault_SetsToCurrentUtcTime()
    {
        // Arrange
        var gradeDto = new CreateGradeDTO(
            Guid.NewGuid(),
            Guid.NewGuid(),
            88.0m,
            default,
            "Well done"
        );

        EnrollmentServiceClientMock
            .Setup(x => x.CheckEnrollmentAsync(gradeDto.StudentId, gradeDto.CourseId))
            .ReturnsAsync(true);

        GradeRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Grade>()))
            .Returns(Task.CompletedTask);

        var before = DateTime.UtcNow;

        // Act
        await GradeService.AddGradeAsync(gradeDto);

        // Assert
        GradeRepositoryMock.Verify(x => x.AddAsync(It.Is<Grade>(g =>
            g.GradedAt >= before &&
            g.GradedAt <= DateTime.UtcNow
        )), Times.Once);
    }
}