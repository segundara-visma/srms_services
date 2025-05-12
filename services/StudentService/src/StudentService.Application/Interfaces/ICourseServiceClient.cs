using StudentService.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace StudentService.Application.Interfaces;

public interface ICourseServiceClient
{
    Task<CourseDTO> GetCourseByIdAsync(Guid courseId);
}