using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminService.Application.Interfaces;

public interface IEnrollmentServiceClient
{
    Task<IEnumerable<object>> GetAllEnrollmentsAsync();
}