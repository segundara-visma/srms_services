using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminService.Application.Services;

public class AdminServiceImpl : IAdminService
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly ITutorServiceClient _tutorServiceClient;
    private readonly IStudentServiceClient _studentServiceClient;
    private readonly IGradeServiceClient _gradeServiceClient;
    private readonly ICourseServiceClient _courseServiceClient;
    private readonly IEnrollmentServiceClient _enrollmentServiceClient;

    public AdminServiceImpl(
        IUserServiceClient userServiceClient,
        ITutorServiceClient tutorServiceClient,
        IStudentServiceClient studentServiceClient,
        IGradeServiceClient gradeServiceClient,
        ICourseServiceClient courseServiceClient,
        IEnrollmentServiceClient enrollmentServiceClient)
    {
        _userServiceClient = userServiceClient ?? throw new ArgumentNullException(nameof(userServiceClient));
        _tutorServiceClient = tutorServiceClient ?? throw new ArgumentNullException(nameof(tutorServiceClient));
        _studentServiceClient = studentServiceClient ?? throw new ArgumentNullException(nameof(studentServiceClient));
        _gradeServiceClient = gradeServiceClient ?? throw new ArgumentNullException(nameof(gradeServiceClient));
        _courseServiceClient = courseServiceClient ?? throw new ArgumentNullException(nameof(courseServiceClient));
        _enrollmentServiceClient = enrollmentServiceClient ?? throw new ArgumentNullException(nameof(enrollmentServiceClient));
    }

    public async Task<IEnumerable<AdminDTO>> GetAllUsersByRoleAsync(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty.", nameof(role));

        return await _userServiceClient.GetUsersByRoleAsync(role);
    }

    public async Task<Guid> CreateUserAsync(string firstName, string lastName, string email, string password, string role)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty.", nameof(role));

        var user = new UserDTO(Guid.NewGuid(), firstName, lastName, email, password, role);
        var userId = await _userServiceClient.CreateUserAsync(user);

        if (role.Equals("Tutor", StringComparison.OrdinalIgnoreCase))
        {
            await _tutorServiceClient.CreateTutorAsync(userId);
        }
        else if (role.Equals("Student", StringComparison.OrdinalIgnoreCase))
        {
            await _studentServiceClient.CreateStudentAsync(userId);
        }

        return userId;
    }

    public async Task<SystemOverviewDTO> GetSystemOverviewAsync()
    {
        var tutorsTask = _tutorServiceClient.GetAllTutorsAsync();
        var studentsTask = _userServiceClient.GetUsersByRoleAsync("Student");
        var gradesTask = _gradeServiceClient.GetAllGradesAsync();
        var coursesTask = _courseServiceClient.GetAllCoursesAsync();
        var enrollmentsTask = _enrollmentServiceClient.GetAllEnrollmentsAsync();

        await Task.WhenAll(tutorsTask, studentsTask, gradesTask, coursesTask, enrollmentsTask);

        var tutors = await tutorsTask;
        var students = await studentsTask;
        var grades = await gradesTask;
        var courses = await coursesTask;
        var enrollments = await enrollmentsTask;

        var averageGrade = grades != null && grades.Any() ? grades.Average(g => g.GradeValue) : 0m;

        return new SystemOverviewDTO(
            TotalTutors: tutors?.Count() ?? 0,
            TotalStudents: students?.Count() ?? 0,
            TotalGrades: grades?.Count() ?? 0,
            TotalCourses: courses?.Count() ?? 0,
            TotalEnrollments: enrollments?.Count() ?? 0,
            AverageGrade: averageGrade
        );
    }

    public async Task AssignCourseToTutorAsync(Guid tutorId, Guid courseId)
    {
        if (tutorId == Guid.Empty)
            throw new ArgumentException("Tutor ID cannot be empty.", nameof(tutorId));
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course ID cannot be empty.", nameof(courseId));

        await _tutorServiceClient.AssignCourseToTutorAsync(tutorId, courseId);
    }

    public async Task<AdminDTO> GetAdminByIdAsync(Guid userId)
    {
        var user = await _userServiceClient.GetUserByIdAsync(userId);
        if (user == null || user.Role != "Admin")
            throw new ArgumentException($"User with ID {userId} is not an admin.");

        return new AdminDTO(user.Id, user.FirstName, user.LastName, user.Email, user.Role, user.Profile);
    }

    public async Task<AdminDTO> UpdateAdminAsync(Guid userId, UpdateRequest request)
    {
        var user = await _userServiceClient.UpdateUserAsync(userId, request);
        if (user == null)
            throw new ArgumentException($"Update request failed.");

        var profile = new Profile
        {
            Address = request.Address,
            Phone = request.Phone,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            Country = request.Country,
            Nationality = request.Nationality,
            Bio = request.Bio,
            FacebookUrl = request.FacebookUrl,
            TwitterUrl = request.TwitterUrl,
            LinkedInUrl = request.LinkedInUrl,
            InstagramUrl = request.InstagramUrl,
            WebsiteUrl = request.WebsiteUrl
        };

        return new AdminDTO(user.Id, user.FirstName, user.LastName, user.Email, user.Role, profile);
    }
}