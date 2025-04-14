using Moq;
using CourseService.Application.DTOs;
using CourseService.Application.Interfaces;
using CourseService.Application.Services;
using CourseService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseService.UnitTests;

public abstract class BaseTest
{
    protected readonly Mock<ICourseRepository> _courseRepositoryMock;
    protected readonly ICourseService _courseService;

    public BaseTest()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _courseService = new CourseServiceImpl(_courseRepositoryMock.Object);
    }

    protected Course CreateTestCourse(Guid? courseId = null)
    {
        return new Course(
            courseId ?? Guid.NewGuid(),
            name: "Mathematics 101",
            code: "MATH101",
            maxStudents: 30
        );
    }

    protected CreateCourseDTO CreateTestCreateCourseDTO()
    {
        return new CreateCourseDTO(
            Name: "Mathematics 101",
            Code: "MATH101",
            MaxStudents: 30
        );
    }

    protected UpdateCourseDTO CreateTestUpdateCourseDTO()
    {
        return new UpdateCourseDTO(
            Name: "Advanced Mathematics",
            Code: "MATH201",
            MaxStudents: 40
        );
    }

    protected CourseDTO CreateTestCourseDTO(Guid id)
    {
        return new CourseDTO(
            Id: id,
            Name: "Mathematics 101",
            Code: "MATH101",
            MaxStudents: 30
        );
    }

    protected void MockGetByIdAsync(Course course)
    {
        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(course.Id)).ReturnsAsync(course);
    }

    protected void MockAddCourseAsync()
    {
        _courseRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Course>())).Returns(Task.CompletedTask);
    }

    protected void MockUpdateCourseAsync()
    {
        _courseRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Course>())).Returns(Task.CompletedTask);
    }

    protected void MockGetAllCoursesAsync(IEnumerable<Course> courses)
    {
        _courseRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(courses);
    }
}