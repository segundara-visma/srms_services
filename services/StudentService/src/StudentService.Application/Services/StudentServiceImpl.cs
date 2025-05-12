using StudentService.Application.DTOs;
using StudentService.Application.Interfaces;
using StudentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentService.Application.Services;

public class StudentServiceImpl : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserServiceClient _userServiceClient;
    private readonly ICourseServiceClient _courseServiceClient;

    public StudentServiceImpl(IStudentRepository studentRepository, IUserServiceClient userServiceClient, ICourseServiceClient courseServiceClient)
    {
        _studentRepository = studentRepository;
        _userServiceClient = userServiceClient;
        _courseServiceClient = courseServiceClient;
    }

    public async Task<StudentDTO> GetStudentByIdAsync(Guid userId)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);
        if (student == null)
            throw new ArgumentException($"Student with User ID {userId} not found.");

        var user = await _userServiceClient.GetUserByIdAsync(userId);
        if (user == null || user.Role != "Student")
            throw new ArgumentException($"User with ID {userId} is not a student.");

        return new StudentDTO(student.Id, user.Id, user.FirstName, user.LastName, user.Email);
    }

    public async Task<IEnumerable<StudentDTO>> GetAllStudentsAsync()
    {
        var users = await _userServiceClient.GetUsersByRoleAsync("Student");
        var studentDTOs = new List<StudentDTO>();

        foreach (var user in users)
        {
            var student = await _studentRepository.GetByUserIdAsync(user.Id);
            if (student != null)
            {
                studentDTOs.Add(new StudentDTO(student.Id, user.Id, user.FirstName, user.LastName, user.Email));
            }
        }

        return studentDTOs;
    }

    public async Task EnrollStudentAsync(Guid userId, Guid courseId)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);
        if (student == null)
        {
            var user = await _userServiceClient.GetUserByIdAsync(userId);
            if (user == null || user.Role != "Student")
                throw new ArgumentException($"User with ID {userId} is not a student.");

            student = new Student(userId);
            await _studentRepository.AddAsync(student);
        }

        var course = await _courseServiceClient.GetCourseByIdAsync(courseId);
        if (course == null)
            throw new ArgumentException($"Course with ID {courseId} not found.");

        student.Enroll(courseId);
        await _studentRepository.UpdateAsync(student);
    }

    public async Task<IEnumerable<CourseDTO>> GetStudentCoursesAsync(Guid userId)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);
        if (student == null)
            throw new ArgumentException($"Student with User ID {userId} not found.");

        var courses = new List<CourseDTO>();
        foreach (var enrollment in student.Enrollments)
        {
            var course = await _courseServiceClient.GetCourseByIdAsync(enrollment.CourseId);
            if (course != null)
            {
                courses.Add(course);
            }
        }

        return courses;
    }
}