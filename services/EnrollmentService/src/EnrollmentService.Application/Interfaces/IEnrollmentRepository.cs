using EnrollmentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEnrollmentRepository
{
    Task<Enrollment> GetByIdAsync(Guid id);
    Task AddAsync(Enrollment enrollment);
    Task<IEnumerable<Enrollment>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<Enrollment>> GetByCourseIdAsync(Guid courseId);
    Task UpdateAsync(Enrollment enrollment);
}