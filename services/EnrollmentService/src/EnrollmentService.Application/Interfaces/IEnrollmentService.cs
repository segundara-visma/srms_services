using EnrollmentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnrollmentService.Application.Interfaces;

public interface IEnrollmentService
{
    Task<EnrollmentDTO> GetEnrollmentByIdAsync(Guid enrollmentId);
    Task<IEnumerable<EnrollmentDTO>> GetEnrollmentsByStudentAsync(Guid studentId);
    Task<IEnumerable<EnrollmentDTO>> GetEnrollmentsByCourseAsync(Guid courseId);
    Task EnrollStudentAsync(Guid studentId, Guid courseId);
    Task CancelEnrollmentAsync(Guid enrollmentId);
    Task<bool> CheckEnrollmentAsync(Guid studentId, Guid courseId);
}