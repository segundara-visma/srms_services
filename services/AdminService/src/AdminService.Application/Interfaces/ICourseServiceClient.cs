using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminService.Application.Interfaces;

public interface ICourseServiceClient
{
    Task<IEnumerable<object>> GetAllCoursesAsync();
}