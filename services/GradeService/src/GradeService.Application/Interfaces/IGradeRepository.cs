using GradeService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GradeService.Application.Interfaces;

public interface IGradeRepository
{
    Task<Grade?> GetByIdAsync(Guid id);
    Task<IEnumerable<Grade>> GetAllAsync();
    Task AddAsync(Grade grade);
}