using StudentService.Application.DTOs;
using StudentService.Application.Interfaces;
using StudentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentService.Application.Services;

public class StudentServiceImpl : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserServiceClient _userServiceClient;

    public StudentServiceImpl(IStudentRepository studentRepository, IUserServiceClient userServiceClient)
    {
        _studentRepository = studentRepository;
        _userServiceClient = userServiceClient;
    }

    public async Task<StudentDTO> GetStudentByIdAsync(Guid userId)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);
        if (student == null)
            throw new ArgumentException($"Student with User ID {userId} not found.");

        var user = await _userServiceClient.GetUserByIdAsync(userId);
        if (user == null || user.Role != "Student")
            throw new ArgumentException($"User with ID {userId} is not a student.");

        return new StudentDTO(student.Id, user.Id, user.FirstName, user.LastName, user.Email, user.Role, user.Profile);
    }

    public async Task<StudentDTO> UpdateStudentAsync(Guid userId, UpdateRequest request)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);
        if (student == null)
            throw new ArgumentException($"Student with User ID {userId} not found.");

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

        return new StudentDTO(student.Id, user.Id, user.FirstName, user.LastName, user.Email, user.Role, profile);
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
                studentDTOs.Add(new StudentDTO(student.Id, user.Id, user.FirstName, user.LastName, user.Email, user.Role, user.Profile));
            }
        }

        return studentDTOs;
    }

    public async Task CreateStudentAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        var existingStudent = await _studentRepository.GetByUserIdAsync(userId);
        if (existingStudent != null)
            throw new InvalidOperationException($"Student with User ID {userId} already exists.");

        var student = new Student(userId); // Use the parameterized constructor

        await _studentRepository.AddAsync(student);
    }
}