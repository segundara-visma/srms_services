using System;
using System.Threading.Tasks;

namespace ReportService.Application.Interfaces;

public interface IEnrollmentServiceClient
{
    Task<bool> CheckEnrollmentAsync(Guid studentId, Guid courseId);
}