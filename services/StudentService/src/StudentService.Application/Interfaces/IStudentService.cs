using StudentService.Application.Common;
using StudentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentService.Application.Interfaces;

public interface IStudentService
{
    Task<StudentDTO> GetStudentByIdAsync(Guid userId);
    //Task<IEnumerable<StudentDTO>> GetAllStudentsAsync();
    Task CreateStudentAsync(Guid userId);
    Task<StudentDTO> UpdateStudentAsync(Guid userId, UpdateRequestDTO request);
    Task<PaginatedResponse<StudentDTO>> GetAllStudentsAsync(int page = 1, int pageSize = 10);
    Task<List<StudentsInBatchDTO>> GetByIdsAsync(List<Guid> ids);
}