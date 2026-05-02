using Moq;
using Moq.Language.Flow;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorService.Application.Common;
using TutorService.Application.Interfaces;
using TutorService.Application.Services;
using TutorService.Domain.Entities;
using Xunit;

namespace TutorService.UnitTests;

public class GetAssignedCoursesTests : BaseTest
{
    private readonly ITutorService _tutorService;

    public GetAssignedCoursesTests()
    {
        _tutorService = new TutorServiceImpl(
            TutorRepositoryMock.Object,
            UserServiceClientMock.Object,
            GradeServiceClientMock.Object);
    }

    [Fact]
    public async Task ReturnsCourses()
    {
        var tutorId = Guid.NewGuid();
        var tutor = CreateTestTutor(userId: tutorId);

        MockGetTutorByUserIdAsync(tutorId, tutor);

        TutorRepositoryMock
            .Setup(x => x.GetPaginatedCoursesByTutorIdAsync(tutor.Id, 1, 10))
            .ReturnsAsync(new PaginatedResult<TutorCourse>
            {
                Items = new List<TutorCourse>
                {
                    new TutorCourse { TutorId = tutor.Id, CourseId = Guid.NewGuid() },
                    new TutorCourse { TutorId = tutor.Id, CourseId = Guid.NewGuid() }
                },
                TotalCount = 2
            });

        var result = await _tutorService.GetAssignedCoursesAsync(tutorId);

        result.Items.Should().HaveCount(2);
    }
}