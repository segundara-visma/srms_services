using EnrollmentService.Application.Interfaces;
using EnrollmentService.Application.Services;
using EnrollmentService.Domain.Entities;
using EnrollmentService.Application.DTOs;
using Moq;
using System;

namespace EnrollmentService.UnitTests;

public class BaseTest
{
    protected readonly Mock<IEnrollmentRepository> MockRepository;
    protected readonly Mock<IUserServiceClient> MockUserClient;
    protected readonly Mock<ICourseServiceClient> MockCourseClient;
    protected readonly EnrollmentServiceImpl Service;

    public BaseTest()
    {
        MockRepository = new Mock<IEnrollmentRepository>();
        MockUserClient = new Mock<IUserServiceClient>();
        MockCourseClient = new Mock<ICourseServiceClient>();
        Service = new EnrollmentServiceImpl(MockRepository.Object, MockUserClient.Object, MockCourseClient.Object);
    }

    // Helper method to create a sample enrollment
    protected Enrollment CreateSampleEnrollment(Guid studentId, Guid courseId)
    {
        return new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = courseId,
            EnrollmentDate = new DateTime(2025, 5, 13, 12, 18, 0, DateTimeKind.Utc).ToLocalTime(), // 12:18 PM EEST, May 13, 2025
            Status = EnrollmentStatus.Enrolled,
            PaymentAmount = null
        };
    }

    // Helper method to create a sample UserDTO
    protected UserDTO CreateSampleUser(Guid id, string firstName = "Test", string lastName = "User", string email = "test@example.com", string role = "Student")
    {
        return new UserDTO(id, firstName, lastName, email, role);
    }

    // Helper method to create a sample CourseDTO
    protected CourseDTO CreateSampleCourse(Guid id, string name = "Sample Course", string code = "CS101", int maxStudents = 30)
    {
        return new CourseDTO(id, name, code, maxStudents);
    }
}