using CourseService.Domain.Entities;
using CourseService.Application.Common;
using CourseService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseService.Application.Interfaces;

public interface ICourseRepository
{
    Task AddAsync(Course course);
    Task<Course?> GetByIdAsync(Guid id);
    Task UpdateAsync(Course course);
    Task<IEnumerable<Course>> GetAllAsync();
    Task<PaginatedResult<Course>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<List<Course>> GetByCourseIdsAsync(List<Guid> ids);
}