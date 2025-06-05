using AdminService.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminService.Application.Interfaces;

public interface IGradeServiceClient
{
    Task<IEnumerable<GradeDTO>> GetAllGradesAsync();
}