using Moq;
using UserService.Application.Interfaces;
using UserService.Application.Services;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Application.Common;

namespace UserService.UnitTests
{
    public class BaseTest
    {
        protected readonly Mock<IUserRepository> _userRepositoryMock;
        protected readonly IUserService _userService;

        public BaseTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserServices(_userRepositoryMock.Object);
        }

        protected User CreateTestUser(Guid? userId = null)
        {
            return new User
            {
                Id = userId ?? Guid.NewGuid(),
                Email = "testuser@example.com",
                Firstname = "Test",
                Lastname = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                RoleId = 1,
                Role = new Role { Id = 1, Name = "Student" },
                Profile = new Profile
                {
                    Address = "123 Test St.",
                    Phone = "123456",
                    City = "TestCity"
                }
            };
        }

        protected Role CreateTestRole(string roleName = "Student")
        {
            return new Role { Id = 1, Name = roleName };
        }

        protected void MockGetByIdAsync(User? user)
        {
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
        }

        protected void MockGetByEmailAsync(User? user)
        {
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
        }

        protected void MockAddUserAsync()
        {
            _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
        }

        protected void MockGetRoleByNameAsync(Role? role)
        {
            _userRepositoryMock.Setup(repo => repo.GetRoleByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(role);
        }

        protected void MockUpdateUserAsync()
        {
            _userRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
        }

        protected void MockGetUsersByRoleIdAsync(int roleId, IEnumerable<User> users)
        {
            _userRepositoryMock.Setup(repo => repo.GetUsersByRoleIdAsync(roleId))
                .ReturnsAsync(users.ToList());
        }

        protected void MockGetUsersByRoleIdPaginated(int roleId, IEnumerable<User> users, int totalCount, int page, int pageSize)
        {
            var result = new PaginatedResult<User>
            {
                Items = users.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = totalCount
            };

            _userRepositoryMock.Setup(repo => repo.GetUsersByRoleIdAsync(roleId, page, pageSize))
                .ReturnsAsync(result);
        }
    }
}