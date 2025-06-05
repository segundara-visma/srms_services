using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminService.Application.Interfaces;

public interface IStudentServiceClient
{
    Task<IEnumerable<object>> GetAllStudentsAsync();
    Task CreateStudentAsync(Guid userId);
}