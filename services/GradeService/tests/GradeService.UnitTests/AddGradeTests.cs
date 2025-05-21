using FluentAssertions;
using GradeService.Application.DTOs;
using GradeService.Application.Services;
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
        var gradeDto = new GradeDTO
        {
            StudentId = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            GradeValue = 88.0m,
            DateGraded = DateTime.UtcNow,
            Comments = "Well done"
        };
        EnrollmentServiceClientMock.Setup(client => client.CheckEnrollmentAsync(gradeDto.StudentId, gradeDto.CourseId))
            .ReturnsAsync(true);
        GradeRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Grade>()))
            .Returns(Task.CompletedTask);

        // Act
        await GradeService.AddGradeAsync(gradeDto);

        // Assert
        GradeRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Grade>(g =>
            g.StudentId == gradeDto.StudentId &&
            g.CourseId == gradeDto.CourseId &&
            g.GradeValue == gradeDto.GradeValue &&
            g.Comments == gradeDto.Comments)), Times.Once());
    }

    [Fact]
    public async Task AddGradeAsync_WhenStudentIsNotEnrolled_ThrowsInvalidOperationException()
    {
        // Arrange
        var gradeDto = new GradeDTO
        {
            StudentId = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            GradeValue = 88.0m,
            DateGraded = DateTime.UtcNow
        };
        EnrollmentServiceClientMock.Setup(client => client.CheckEnrollmentAsync(gradeDto.StudentId, gradeDto.CourseId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => GradeService.AddGradeAsync(gradeDto));
        GradeRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Grade>()), Times.Never());
    }

    [Fact]
    public async Task AddGradeAsync_WhenGradeValueIsOutOfRange_ThrowsArgumentException()
    {
        // Arrange
        var gradeDto = new GradeDTO
        {
            StudentId = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            GradeValue = 101m, // Out of range
            DateGraded = DateTime.UtcNow
        };
        EnrollmentServiceClientMock.Setup(client => client.CheckEnrollmentAsync(gradeDto.StudentId, gradeDto.CourseId))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => GradeService.AddGradeAsync(gradeDto));
        GradeRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Grade>()), Times.Never());
    }

    [Fact]
    public async Task AddGradeAsync_WhenGradeDtoIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        GradeDTO gradeDto = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => GradeService.AddGradeAsync(gradeDto));
        GradeRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Grade>()), Times.Never());
    }

    [Fact]
    public async Task AddGradeAsync_WhenDateGradedIsNotSpecified_SetsToCurrentUtcTime()
    {
        // Arrange
        var gradeDto = new GradeDTO
        {
            StudentId = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            GradeValue = 88.0m,
            DateGraded = default, // Not specified
            Comments = "Well done"
        };
        EnrollmentServiceClientMock.Setup(client => client.CheckEnrollmentAsync(gradeDto.StudentId, gradeDto.CourseId))
            .ReturnsAsync(true);
        GradeRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Grade>()))
            .Returns(Task.CompletedTask);

        var beforeTestTime = DateTime.UtcNow;

        // Act
        await GradeService.AddGradeAsync(gradeDto);

        // Assert
        GradeRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Grade>(g =>
            g.DateGraded >= beforeTestTime &&
            g.DateGraded <= DateTime.UtcNow)), Times.Once());
    }
}