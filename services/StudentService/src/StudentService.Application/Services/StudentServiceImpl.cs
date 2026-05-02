using StudentService.Application.Common;
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

    public async Task<StudentDTO> UpdateStudentAsync(Guid userId, UpdateRequestDTO request)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);
        if (student == null)
            throw new ArgumentException($"Student with User ID {userId} not found.");

        var user = await _userServiceClient.UpdateUserAsync(userId, request);
        if (user == null)
            throw new ArgumentException($"Update request failed.");

        var profile = new ProfileDTO
        (
            request.Address,
            request.Phone,
            request.City,
            request.State,
            request.ZipCode,
            request.Country,
            request.Nationality,
            request.Bio,
            request.FacebookUrl,
            request.TwitterUrl,
            request.LinkedInUrl,
            request.InstagramUrl,
            request.WebsiteUrl
        );

        return new StudentDTO(student.Id, user.Id, user.FirstName, user.LastName, user.Email, user.Role, profile);
    }

    //public async Task<IEnumerable<StudentDTO>> GetAllStudentsAsync()
    //{
    //    var users = await _userServiceClient.GetUsersByRoleAsync("Student");
    //    var studentDTOs = new List<StudentDTO>();

    //    foreach (var user in users)
    //    {
    //        var student = await _studentRepository.GetByUserIdAsync(user.Id);
    //        if (student != null)
    //        {
    //            studentDTOs.Add(new StudentDTO(student.Id, user.Id, user.FirstName, user.LastName, user.Email, user.Role, user.Profile));
    //        }
    //    }

    //    return studentDTOs;
    //}

    public async Task<PaginatedResponse<StudentDTO>> GetAllStudentsAsync(int page = 1, int pageSize = 10)
    {
        var pagedUsers = await _userServiceClient.GetUsersByRoleAsync("Student", page, pageSize);

        var users = pagedUsers?.Items?.ToList() ?? new List<UserDTO>();

        var userIds = users.Select(u => u.Id).ToList();

        var students = await _studentRepository.GetByUserIdsAsync(userIds)
                      ?? new List<Student>();

        if (!students.Any())
        {
            return new PaginatedResponse<StudentDTO>
            {
                Items = new List<StudentDTO>(),
                TotalCount = 0,
                Page = page,
                PageSize = pageSize
            };
        }

        var studentMap = students
            .Where(s => s != null)
            .GroupBy(s => s.UserId)
            .ToDictionary(g => g.Key, g => g.First());

        var studentDTOs = users
            .Where(u => studentMap.ContainsKey(u.Id))
            .Select(u =>
            {
                var student = studentMap[u.Id];

                return new StudentDTO(
                    student.Id,
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Role,
                    u.Profile
                );
            })
            .ToList();

        return new PaginatedResponse<StudentDTO>
        {
            Items = studentDTOs,
            TotalCount = pagedUsers?.TotalCount ?? studentDTOs.Count,
            Page = pagedUsers?.Page ?? page,
            PageSize = pagedUsers?.PageSize ?? pageSize
        };
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

    public async Task<List<StudentsInBatchDTO>> GetByIdsAsync(List<Guid> ids)
    {
        var students = await _studentRepository.GetByUserIdsAsync(ids);

        return students.Select(s => new StudentsInBatchDTO(
            s.Id,
            s.UserId
        )).ToList();
    }
}