using GradeService.Application.Common;
using GradeService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GradeService.Application.Interfaces;

public interface IGradeService
{
    Task<GradeDTO?> GetGradeByIdAsync(Guid id);
    Task<IEnumerable<GradeDTO>> GetAllGradesAsync();
    Task<Guid> AddGradeAsync(CreateGradeDTO gradeDto);
    Task<IEnumerable<GradeDTO>> GetGradesByStudentAsync(Guid studentId);
    Task AssignGradeAsync(Guid studentId, Guid courseId, decimal grade);
    Task<PaginatedResponse<GradeDTO>> GetGradesByStudentAsync(Guid userId, int page = 1, int pageSize = 10);
    Task<PaginatedResponse<GradeDTO>> GetGradesByCourseAsync(Guid courseId, int page = 1, int pageSize = 10);
}