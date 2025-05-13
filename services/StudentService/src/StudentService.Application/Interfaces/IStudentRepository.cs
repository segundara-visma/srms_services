using StudentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentService.Application.Interfaces;

public interface IStudentRepository
{
    Task<Student> GetByIdAsync(Guid id);
    Task<Student> GetByUserIdAsync(Guid userId);
    Task AddAsync(Student student);
    Task UpdateAsync(Student student);
    //Task DeleteAsync(Guid studentId);
    Task<IEnumerable<Student>> GetAllAsync();
}