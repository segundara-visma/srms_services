using EnrollmentService.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace EnrollmentService.Application.Interfaces;

public interface ICourseServiceClient
{
    Task<CourseDTO> GetCourseByIdAsync(Guid courseId);
}