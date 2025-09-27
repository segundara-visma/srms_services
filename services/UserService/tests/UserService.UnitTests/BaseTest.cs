using Moq;
using UserService.Application.Interfaces;
using UserService.Application.Services;
using UserService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // Helper function to create a test user with initialized Role and Profile
        protected User CreateTestUser(Guid? userId = null)
        {
            return new User
            {
                Id = userId ?? Guid.NewGuid(),
                Email = "testuser@example.com",
                Firstname = "Test",
                Lastname = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                RoleId = 1, // int for RoleId
                Role = new Role { Id = 1, Name = "Student" }, // Initialize Role
                Profile = new Profile { Address = "123 Test St." } // Initialize Profile
            };
        }

        // Helper function to create a test role
        protected Role CreateTestRole(string roleName = "Student")
        {
            return new Role { Id = 1, Name = roleName }; // int for Role.Id
        }

        // Mock repository methods
        protected void MockGetByIdAsync(User user)
        {
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);
        }

        protected void MockGetByEmailAsync(User user)
        {
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        }

        protected void MockAddUserAsync()
        {
            _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        }

        protected void MockGetRoleByNameAsync(Role role)
        {
            _userRepositoryMock.Setup(repo => repo.GetRoleByNameAsync(role.Name)).ReturnsAsync(role);
        }

        protected void MockUpdateUserAsync(User user)
        {
            _userRepositoryMock.Setup(repo => repo.UpdateAsync(user)).Returns(Task.CompletedTask);
        }

        protected void MockGetUsersByRoleIdAsync(int roleId, IEnumerable<User> users, int totalCount, int page = 1, int pageSize = 10)
        {
            var paginatedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var paginatedResult = new PaginatedResult<User> { Items = paginatedUsers, TotalCount = totalCount };
            _userRepositoryMock.Setup(repo => repo.GetUsersByRoleIdAsync(roleId, page, pageSize))
                .Returns(Task.FromResult(paginatedResult));
        }
    }
}