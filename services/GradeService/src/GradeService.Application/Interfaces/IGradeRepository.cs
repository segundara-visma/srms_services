using GradeService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GradeService.Application.Common;

namespace GradeService.Application.Interfaces;

public interface IGradeRepository
{
    Task<Grade?> GetByIdAsync(Guid id);
    Task<IEnumerable<Grade>> GetAllAsync();
    Task AddAsync(Grade grade);
    Task<IEnumerable<Grade>> GetByStudentIdAsync(Guid studentId);
    Task<PaginatedResult<Grade>> GetWithPaginationByStudentIdAsync(Guid userId, int page = 1, int pageSize = 10);
    Task<PaginatedResult<Grade>> GetWithPaginationByCourseIdAsync(Guid courseId, int page = 1, int pageSize = 10);
}