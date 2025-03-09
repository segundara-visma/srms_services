using Moq;
using UserService.Application.Interfaces;
using UserService.Application.Services;
using UserService.Domain.Entities;
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

        // Helper function to create a test user
        protected User CreateTestUser(Guid? userId = null)
        {
            return new User
            {
                Id = userId ?? Guid.NewGuid(),
                Email = "testuser@example.com",
                Firstname = "Test",
                Lastname = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = new Role { Id = 1, Name = "Student" },
                Profile = new Profile { Address = "123 Test St." }
            };
        }

        // Helper function to create a test role
        protected Role CreateTestRole(string roleName = "Student")
        {
            return new Role { Id = 1, Name = roleName };
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

        //// Mock ValidatePasswordAsync in the UserService (this is specific to the service)
        //protected void MockValidatePasswordAsync(User user, bool result)
        //{
        //    // Mock the GetByIdAsync method of the repository, which is used in the service
        //    _userRepositoryMock.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

        //    // We do not mock the ValidatePasswordAsync method on IUserService directly.
        //    // Instead, we expect that _userService will call GetByIdAsync via _userRepositoryMock.
        //    // The actual ValidatePasswordAsync is tested by interacting with the service.
        //}

        // Mock UpdateAsync in UserRepository
        protected void MockUpdateUserAsync()
        {
            _userRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        }
    }
}
