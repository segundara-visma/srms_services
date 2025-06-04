using System;
using System.Threading.Tasks;

namespace TutorService.Application.Interfaces;

public interface IGradeServiceClient
{
    Task<bool> AssignGradeAsync(Guid studentId, Guid courseId, decimal grade);
}