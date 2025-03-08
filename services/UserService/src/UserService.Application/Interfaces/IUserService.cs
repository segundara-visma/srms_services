using UserService.Domain.Entities;
using System;
using System.Threading.Tasks;
using UserService.Application.DTOs;

namespace UserService.Application.Interfaces;

public interface IUserService
{
    Task RegisterUser(User user, string password, string role);  // This method handles the business logic for user registration
}
