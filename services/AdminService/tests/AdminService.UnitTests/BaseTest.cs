using Moq;
using AdminService.Application.Common;
using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    // =========================
    // DTO FACTORIES
    // =========================

    protected AdminDTO CreateTestAdminDTO(Guid userId, ProfileDTO? profile = null)
        => new(userId, "John", "Doe", "john.doe@example.com", "Admin", profile);

    protected UserDTO CreateTestUserDTO(Guid userId, string role = "Admin")
        => new(userId, "John", "Doe", "john.doe@example.com", "123", role);

    protected UpdateRequestDTO CreateTestUpdateRequest(
        Guid id,
        string? address = null,
        string? phone = null)
        => new(
            id,
            "Jane",
            "Doe",
            "jane.doe@example.com",
            address,
            phone,
            null, null, null, null, null,
            null, null, null, null, null, null
        );

    // =========================
    // MOCKS
    // =========================

    protected void MockGetUserByIdAsync(Guid userId, AdminDTO? adminDTO)
    {
        UserServiceClientMock
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(adminDTO);
    }

    protected void MockUpdateUserAsync(Guid userId, UpdateRequestDTO request, AdminDTO? adminDTO)
    {
        UserServiceClientMock
            .Setup(x => x.UpdateUserAsync(userId, It.IsAny<UpdateRequestDTO>()))
            .ReturnsAsync(adminDTO);
    }

    // FIXED PAGINATION MOCK (IMPORTANT FIX)
    protected void MockGetUsersByRoleAsync(
        string role,
        IEnumerable<AdminDTO> admins,
        int page = 1,
        int pageSize = 10,
        int totalCount = 0)
    {
        UserServiceClientMock
            .Setup(x => x.GetUsersByRoleAsync(role, page, pageSize))
            .ReturnsAsync(new PaginatedResponse<AdminDTO>
            {
                Items = admins,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
    }

    protected void MockGetAllUsersByRoleAsync(string role, IEnumerable<AdminDTO> admins)
    {
        UserServiceClientMock
            .Setup(x => x.GetAllUsersByRoleAsync(role))
            .ReturnsAsync(admins);
    }

    protected void MockGetAllGradesAsync(IEnumerable<GradeDTO> grades)
    {
        GradeServiceClientMock
            .Setup(x => x.GetAllGradesAsync())
            .ReturnsAsync(grades);
    }

    protected void MockGetAllCoursesAsync(IEnumerable<object> courses)
    {
        CourseServiceClientMock
            .Setup(x => x.GetAllCoursesAsync())
            .ReturnsAsync(courses);
    }

    protected void MockGetAllEnrollmentsAsync(IEnumerable<object> enrollments)
    {
        EnrollmentServiceClientMock
            .Setup(x => x.GetAllEnrollmentsAsync())
            .ReturnsAsync(enrollments);
    }

    protected void MockAssignCourseToTutorAsync(Guid tutorId, Guid courseId)
    {
        TutorServiceClientMock
            .Setup(x => x.AssignCourseToTutorAsync(tutorId, courseId))
            .Returns(Task.CompletedTask);
    }

    protected void MockCreateUserAsync(UserDTO user, Guid userId)
    {
        UserServiceClientMock
            .Setup(x => x.CreateUserAsync(It.IsAny<UserDTO>()))
            .ReturnsAsync(userId);
    }

    protected void MockTutorCreateAsync(Guid userId)
    {
        TutorServiceClientMock
            .Setup(x => x.CreateTutorAsync(userId))
            .Returns(Task.CompletedTask);
    }

    protected void MockStudentCreateAsync(Guid userId)
    {
        StudentServiceClientMock
            .Setup(x => x.CreateStudentAsync(userId))
            .Returns(Task.CompletedTask);
    }
}