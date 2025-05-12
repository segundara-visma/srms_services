using StudentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentService.Application.Interfaces;

public interface IStudentService
{
    Task<StudentDTO> GetStudentByIdAsync(Guid userId);
    Task<IEnumerable<StudentDTO>> GetAllStudentsAsync();
    Task EnrollStudentAsync(Guid userId, Guid courseId);
    Task<IEnumerable<CourseDTO>> GetStudentCoursesAsync(Guid userId);
}