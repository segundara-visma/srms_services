using Moq;
using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;
using System;
using System.Collections.Generic;

namespace AdminService.UnitTests
{
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

        protected UserDTO CreateTestUserDTO(Guid userId, string role = "Admin")
        {
            return new UserDTO(userId, "John", "Doe", "john.doe@example.com", "123", role);
        }

        protected AdminDTO CreateTestAdminDTO(Guid userId, Profile? profile = null)
        {
            return new AdminDTO(userId, "John", "Doe", "john.doe@example.com", "Admin", profile);
        }

        protected UpdateRequest CreateTestUpdateRequest(Guid id, string? address = null, string? phone = null)
        {
            return new UpdateRequest(id, "Jane", "Doe", "jane.doe@example.com",
                address, phone, null, null, null, null, null, null, null, null, null, null, null);
        }

        protected void MockGetUserByIdAsync(Guid userId, AdminDTO adminDTO)
        {
            UserServiceClientMock.Setup(c => c.GetUserByIdAsync(userId)).ReturnsAsync(adminDTO);
        }

        protected void MockCreateUserAsync(UserDTO user, Guid userId)
        {
            UserServiceClientMock.Setup(c => c.CreateUserAsync(It.Is<UserDTO>(u =>
                u.FirstName == user.FirstName &&
                u.LastName == user.LastName &&
                u.Email == user.Email &&
                u.Password == user.Password &&
                u.Role == user.Role)))
                .ReturnsAsync(userId);
        }

        protected void MockGetUsersByRoleAsync(string role, IEnumerable<AdminDTO> admins)
        {
            UserServiceClientMock.Setup(c => c.GetUsersByRoleAsync(role)).ReturnsAsync(admins);
        }

        protected void MockUpdateUserAsync(Guid userId, UpdateRequest request, AdminDTO adminDTO)
        {
            UserServiceClientMock.Setup(c => c.UpdateUserAsync(userId, It.Is<UpdateRequest>(r => r.Email == request.Email)))
                .ReturnsAsync(adminDTO);
        }

        protected void MockTutorCreateAsync(Guid userId)
        {
            TutorServiceClientMock.Setup(c => c.CreateTutorAsync(userId)).Returns(Task.CompletedTask);
        }

        protected void MockStudentCreateAsync(Guid userId)
        {
            StudentServiceClientMock.Setup(c => c.CreateStudentAsync(userId)).Returns(Task.CompletedTask);
        }

        protected void MockAssignCourseToTutorAsync(Guid tutorId, Guid courseId)
        {
            TutorServiceClientMock.Setup(c => c.AssignCourseToTutorAsync(tutorId, courseId)).Returns(Task.CompletedTask);
        }

        protected void MockGetAllTutorsAsync(IEnumerable<TutorDTO> tutors)
        {
            TutorServiceClientMock.Setup(c => c.GetAllTutorsAsync()).ReturnsAsync(tutors);
        }

        protected void MockGetAllGradesAsync(IEnumerable<GradeDTO> grades)
        {
            GradeServiceClientMock.Setup(c => c.GetAllGradesAsync()).ReturnsAsync(grades);
        }

        protected void MockGetAllCoursesAsync(IEnumerable<object> courses)
        {
            CourseServiceClientMock.Setup(c => c.GetAllCoursesAsync()).ReturnsAsync(courses);
        }

        protected void MockGetAllEnrollmentsAsync(IEnumerable<object> enrollments)
        {
            EnrollmentServiceClientMock.Setup(c => c.GetAllEnrollmentsAsync()).ReturnsAsync(enrollments);
        }
    }
}