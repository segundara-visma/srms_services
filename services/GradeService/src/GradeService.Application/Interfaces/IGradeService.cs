using GradeService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GradeService.Application.Interfaces;

public interface IGradeService
{
    Task<GradeDTO?> GetGradeByIdAsync(Guid id);
    Task<IEnumerable<GradeDTO>> GetAllGradesAsync();
    Task AddGradeAsync(GradeDTO gradeDto);
    Task<IEnumerable<GradeDTO>> GetGradesByStudentAsync(Guid studentId);
}