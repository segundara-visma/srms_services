using Xunit;
using Moq;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Application.Interfaces;
using System.Threading.Tasks;

public class UserServiceTests
{
    [Fact]
    public async Task RegisterUser_ShouldCreateUser_WhenDataIsValid()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new User("test@example.com", "hashedpassword");
        userRepositoryMock.Setup(repo => repo.CreateAsync(user)).ReturnsAsync(user);

        // Act
        var result = await userService.RegisterUser(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }
}
