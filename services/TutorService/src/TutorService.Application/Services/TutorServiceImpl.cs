using TutorService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutorService.Domain.Entities;
using TutorService.Application.Interfaces;

namespace TutorService.Application.Services;

public class TutorServiceImpl : ITutorService
{
    private readonly ITutorRepository _tutorRepository;
    private readonly IUserServiceClient _userServiceClient;
    private readonly IGradeServiceClient _gradeServiceClient;

    public TutorServiceImpl(
        ITutorRepository tutorRepository,
        IUserServiceClient userServiceClient,
        IGradeServiceClient gradeServiceClient)
    {
        _tutorRepository = tutorRepository ?? throw new ArgumentNullException(nameof(tutorRepository));
        _userServiceClient = userServiceClient ?? throw new ArgumentNullException(nameof(userServiceClient));
        _gradeServiceClient = gradeServiceClient ?? throw new ArgumentNullException(nameof(gradeServiceClient));
    }

    public async Task<TutorDTO> GetTutorByIdAsync(Guid tutorId)
    {
        var tutor = await _tutorRepository.GetByUserIdAsync(tutorId);
        if (tutor == null)
            throw new ArgumentException($"Tutor with ID {tutorId} not found.");

        var user = await _userServiceClient.GetUserByIdAsync(tutorId);
        if (user == null || user.Role != "Tutor")
            throw new ArgumentException($"User with ID {tutorId} is not a tutor.");

        //return new TutorDTO
        //{
        //    Id = tutor.Id,
        //    UserId = user.Id,
        //    FirstName = user.FirstName,
        //    LastName = user.LastName,
        //    Email = user.Email
        //};
        return new TutorDTO(tutor.Id, user.Id, user.FirstName, user.LastName, user.Email, user.Role, user.Profile);
    }

    public async Task<TutorDTO> UpdateTutorAsync(Guid tutorId, UpdateRequest request)
    {
        var tutor = await _tutorRepository.GetByUserIdAsync(tutorId);
        if (tutor == null)
            throw new ArgumentException($"Tutor with ID {tutorId} not found.");

        var user = await _userServiceClient.UpdateUserAsync(tutorId, request);
        if (user == null)
            throw new ArgumentException($"Update request failed.");

        //return new TutorDTO
        //{
        //    Id = tutor.Id,
        //    UserId = user.Id,
        //    FirstName = user.FirstName,
        //    LastName = user.LastName,
        //    Email = user.Email
        //};

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

        return new TutorDTO(tutor.Id, user.Id, user.FirstName, user.LastName, user.Email, user.Role, profile);
    }

    public async Task<IEnumerable<TutorDTO>> GetAllTutorsAsync()
    {
        var users = await _userServiceClient.GetUsersByRoleAsync("Tutor");
        var tutorDTOs = new List<TutorDTO>();

        foreach (var user in users)
        {
            var tutor = await _tutorRepository.GetByUserIdAsync(user.Id);
            if (tutor != null)
            {
                tutorDTOs.Add(new TutorDTO(tutor.Id, user.Id, user.FirstName, user.LastName, user.Email, user.Role, user.Profile));
                //{
                //    Id = tutor.Id,
                //    UserId = user.Id,
                //    FirstName = user.FirstName,
                //    LastName = user.LastName,
                //    Email = user.Email
                //});
            }
        }

        return tutorDTOs;
    }

    public async Task<bool> AssignGradeAsync(Guid studentId, Guid courseId, decimal grade)
    {
        if (grade < 0 || grade > 100)
            throw new ArgumentException("Grade must be between 0 and 100.", nameof(grade));

        return await _gradeServiceClient.AssignGradeAsync(studentId, courseId, grade);
    }

    public async Task<IEnumerable<Guid>> GetAssignedCoursesAsync(Guid tutorId)
    {
        var tutor = await _tutorRepository.GetByUserIdAsync(tutorId);
        if (tutor == null)
            throw new ArgumentException($"Tutor with ID {tutorId} not found.");

        var courses = await _tutorRepository.GetCoursesByTutorIdAsync(tutorId);
        return courses.Select(tc => tc.CourseId);
    }

    public async Task AssignCourseToTutorAsync(Guid tutorId, Guid courseId)
    {
        var tutor = await _tutorRepository.GetByUserIdAsync(tutorId);
        if (tutor == null)
            throw new ArgumentException($"Tutor with ID {tutorId} not found.");

        var existingCourse = await _tutorRepository.GetCoursesByTutorIdAsync(tutorId);
        if (existingCourse.Any(tc => tc.CourseId == courseId))
            throw new ArgumentException($"Tutor {tutorId} is already assigned to course {courseId}.");

        var tutorCourse = new TutorCourse
        {
            Id = Guid.NewGuid(),
            TutorId = tutorId,
            CourseId = courseId,
            AssignmentDate = DateTime.UtcNow
        };
        await _tutorRepository.AddTutorCourseAsync(tutorCourse);
    }

    public async Task CreateTutorAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        var existingTutor = await _tutorRepository.GetByUserIdAsync(userId);
        if (existingTutor != null)
            throw new InvalidOperationException($"Tutor with User ID {userId} already exists.");

        var user = await _userServiceClient.GetUserByIdAsync(userId);
        if (user == null)
            throw new ArgumentException($"User with ID {userId} not found.", nameof(userId));

        if (!string.Equals(user.Role, "Tutor", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"User with ID {userId} does not have the 'Tutor' role.");

        var tutor = new Tutor
        {
            Id = Guid.NewGuid(),
            UserId = userId
        };

        await _tutorRepository.AddAsync(tutor);
    }
}