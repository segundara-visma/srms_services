using System;
using System.Threading.Tasks;

namespace ReportService.Application.Interfaces;

public interface ICourseServiceClient
{
    Task<(string Title, int Credits)?> GetCourseDetailsAsync(Guid courseId);
}