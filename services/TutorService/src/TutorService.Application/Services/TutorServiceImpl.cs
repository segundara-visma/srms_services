using TutorService.Application.Common;
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

        return new TutorDTO(tutor.Id, user.Id, user.FirstName, user.LastName, user.Email, user.Role, user.Profile);
    }

    public async Task<TutorDTO> UpdateTutorAsync(Guid tutorId, UpdateRequestDTO request)
    {
        var tutor = await _tutorRepository.GetByUserIdAsync(tutorId);
        if (tutor == null)
            throw new ArgumentException($"Tutor with ID {tutorId} not found.");

        var user = await _userServiceClient.UpdateUserAsync(tutorId, request);
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

        return new TutorDTO(tutor.Id, user.Id, user.FirstName, user.LastName, user.Email, user.Role, profile);
    }

    public async Task<PaginatedResponse<TutorDTO>> GetAllTutorsAsync(int page = 1, int pageSize = 10)
    {
        // Step 1: Get paginated users
        var pagedUsers = await _userServiceClient.GetUsersByRoleAsync("Tutor", page, pageSize);

        var userIds = pagedUsers.Items.Select(u => u.Id).ToList();

        // Step 2: Get tutors in bulk
        var tutors = await _tutorRepository.GetByUserIdsAsync(userIds);

        var tutorMap = tutors.ToDictionary(s => s.UserId);

        // Step 3: Merge data
        var tutorDTOs = pagedUsers.Items
            .Where(u => tutorMap.ContainsKey(u.Id))
            .Select(u =>
            {
                var tutor = tutorMap[u.Id];

                return new TutorDTO(
                    tutor.Id,
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Role,
                    u.Profile
                );
            })
            .ToList();

        // Step 4: Return unified pagination
        return new PaginatedResponse<TutorDTO>
        {
            Items = tutorDTOs,
            TotalCount = pagedUsers.TotalCount,
            Page = pagedUsers.Page,
            PageSize = pagedUsers.PageSize
        };
    }

    public async Task<bool> AssignGradeAsync(Guid studentId, Guid courseId, decimal grade)
    {
        if (grade < 0 || grade > 100)
            throw new ArgumentException("Grade must be between 0 and 100.", nameof(grade));

        return await _gradeServiceClient.AssignGradeAsync(studentId, courseId, grade);
    }

    public async Task<PaginatedResponse<TutorCoursesDTO>> GetAssignedCoursesAsync(Guid userId, int page = 1, int pageSize = 10)
    {
        var tutor = await _tutorRepository.GetByUserIdAsync(userId);
        if (tutor == null)
            throw new ArgumentException($"Tutor with ID {userId} not found.");

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var paginatedResult = await _tutorRepository.GetPaginatedCoursesByTutorIdAsync(tutor.Id, page, pageSize);

        var items = paginatedResult.Items.Select(tc => new TutorCoursesDTO(tc.Id, tc.TutorId, tc.CourseId));

        return new PaginatedResponse<TutorCoursesDTO>
        {
            Items = items,
            TotalCount = paginatedResult.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task AssignCourseToTutorAsync(Guid tutorId, Guid courseId)
    {
        var tutor = await _tutorRepository.GetByUserIdAsync(tutorId);
        if (tutor == null)
            throw new ArgumentException($"Tutor with ID {tutorId} not found.");

        var existingCourse = await _tutorRepository.GetCoursesByTutorIdAsync(tutor.Id);
        if (existingCourse.Any(tc => tc.CourseId == courseId))
            throw new ArgumentException($"Tutor {tutorId} is already assigned to course {courseId}.");

        var tutorCourse = new TutorCourse
        {
            Id = Guid.NewGuid(),
            TutorId = tutor.Id,
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