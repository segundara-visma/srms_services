using TutorService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutorService.Domain.Entities;
using TutorService.Application.Interfaces;

namespace TutorService.Application.Services
{
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

            return new TutorDTO
            {
                Id = tutor.Id,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
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
                    tutorDTOs.Add(new TutorDTO
                    {
                        Id = tutor.Id,
                        UserId = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email
                    });
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
    }
}