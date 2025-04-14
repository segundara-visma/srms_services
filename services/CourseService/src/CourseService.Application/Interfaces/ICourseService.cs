using CourseService.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CourseService.Application.DTOs;

namespace CourseService.Application.Interfaces;

public interface ICourseService
{
    //Task<Guid> CreateCourseAsync(string name, string code, int maxStudents);
    //Task<Course> GetCourseByIdAsync(Guid id);
    //Task UpdateCourseAsync(Guid id, string name, string code, int maxStudents);
    //Task<IEnumerable<Course>> GetAllCoursesAsync();

    Task<Guid> CreateCourseAsync(CreateCourseDTO dto);
    Task<CourseDTO> GetCourseByIdAsync(Guid id);
    Task UpdateCourseAsync(Guid id, UpdateCourseDTO dto);
    Task<IEnumerable<CourseDTO>> GetAllCoursesAsync();
}
