using CourseService.Application.Common;
using CourseService.Application.DTOs;
using CourseService.Application.Interfaces;
using CourseService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseService.Application.Services;

public class CourseServiceImpl : ICourseService
{
    private readonly ICourseRepository _repository;

    public CourseServiceImpl(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CreateCourseAsync(CreateCourseDTO dto)
    {
        if (dto.MaxStudents < 0)
            throw new ArgumentException("MaxStudents cannot be negative.");

        var course = new Course(dto.Name, dto.Code, dto.MaxStudents);
        await _repository.AddAsync(course);
        return course.Id;
    }

    public async Task<CourseDTO?> GetCourseByIdAsync(Guid id)
    {
        var course = await _repository.GetByIdAsync(id);
        if (course == null)
            return null;

        return new CourseDTO(course.Id, course.Name, course.Code, course.MaxStudents);
    }

    public async Task UpdateCourseAsync(Guid id, UpdateCourseDTO dto)
    {
        var course = await _repository.GetByIdAsync(id);
        if (course == null)
            throw new ArgumentException($"Course with ID {id} not found.");

        if (dto.MaxStudents < 0)
            throw new ArgumentException("MaxStudents cannot be negative.");

        course.Update(dto.Name, dto.Code, dto.MaxStudents);
        await _repository.UpdateAsync(course);
    }

    public async Task<IEnumerable<CourseDTO>> GetAllCoursesAsync()
    {
        var courses = await _repository.GetAllAsync();
        return courses.Select(course => new CourseDTO(course.Id, course.Name, course.Code, course.MaxStudents));
    }

    public async Task<PaginatedResponse<CourseDTO>> GetAllCoursesWithPaginationAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var paginatedResult = await _repository.GetAllWithPaginationAsync(page, pageSize);

        var items = paginatedResult.Items.Select(course => new CourseDTO(course.Id, course.Name, course.Code, course.MaxStudents));

        return new PaginatedResponse<CourseDTO>
        {
            Items = items,
            TotalCount = paginatedResult.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<List<CoursesInBatchDTO>> GetByIdsAsync(List<Guid> ids)
    {
        var courses = await _repository.GetByCourseIdsAsync(ids);

        return courses.Select(c => new CoursesInBatchDTO(
            c.Id,
            c.Name,
            c.Code
        )).ToList();
    }
}