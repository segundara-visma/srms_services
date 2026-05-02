using EnrollmentService.Domain.Entities;
using EnrollmentService.Application.Common;
using EnrollmentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(Guid id);
    Task AddAsync(Enrollment enrollment);
    //Task<IEnumerable<Enrollment>> GetByStudentIdAsync(Guid studentId);
    Task<PaginatedResult<Enrollment>> GetByStudentIdAsync(Guid studentId, int page = 1, int pageSize = 10);
    //Task<IEnumerable<Enrollment>> GetByCourseIdAsync(Guid courseId);
    Task<PaginatedResult<Enrollment>> GetByCourseIdAsync(Guid courseId, int page = 1, int pageSize = 10);
    Task UpdateAsync(Enrollment enrollment);
    Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId);
    Task<IEnumerable<Enrollment>> GetAllAsync();
}