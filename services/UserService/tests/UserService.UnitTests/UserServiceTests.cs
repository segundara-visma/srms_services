using System;
using System.Threading.Tasks;
using UserService.Application.Interfaces;
using UserService.Application.Services;
using UserService.Domain.Entities;
using Moq;
using Xunit;
using FluentAssertions;
using BCrypt.Net;  // Add this to handle password hashing
using Microsoft.AspNetCore.Http;  // For mocking cookies
using Xunit.Abstractions;

namespace UserService.UnitTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IUserService _userService;
        private readonly ITestOutputHelper _output;  // Add this line to capture output

        // Constructor to initialize the mocks
        public UserServiceTests(ITestOutputHelper output)
        {
            _output = output;  // Assign to the private field
            _userRepositoryMock = new Mock<IUserRepository>();

            _userService = new UserServices(_userRepositoryMock.Object);
        }

        // Test case for successful user registration
        [Fact]
        public async Task RegisterUser_Should_Create_New_User_When_Valid()
        {
            // Arrange
            var rawPassword = "password123";  // raw password input by the user
            var roleName = "Student";  // the user's role
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);  // Hash the password for storage
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = hashedPassword,
                Firstname = "First",
                Lastname = "Last",
                // Role = "Student" 
            };

            // Mock behavior: GetByEmailAsync returns null, indicating no user exists with this email
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(newUser.Email)).ReturnsAsync((User?)null);  // Ensure null is properly casted as User?

            // Mock behavior: AddAsync completes without doing anything
            _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Mock the role fetching (assuming you have a method GetRoleByNameAsync)
            var mockRole = new Role { Id = 3, Name = "Student" };  // Mock the role object
            _userRepositoryMock.Setup(repo => repo.GetRoleByNameAsync(roleName)).ReturnsAsync(mockRole);  // Mock role retrieval

            // Act
            await _userService.RegisterUser(newUser, rawPassword, roleName);  // Pass the user object and the rawPassword

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
        }
    }
}
