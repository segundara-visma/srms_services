using System;
using Moq;
using Xunit;
using AuthService.Application.Interfaces;
using AuthService.Application.Helpers;
using AuthService.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using AuthService.Domain.Entities;
using Xunit.Abstractions;
using System.Security.Claims;

namespace AuthService.UnitTests
{
    public class BaseTestClass
    {
        protected readonly Mock<IAuthRepository> _authRepositoryMock;
        protected readonly Mock<IUserService> _userServiceMock;
        protected readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        protected readonly Mock<JWTTokenHelper> _jwtTokenHelperMock;
        protected readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        protected readonly Mock<IDistributedCache> _cache;
        protected readonly IAuthService _authService;

        public BaseTestClass()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _userServiceMock = new Mock<IUserService>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _jwtTokenHelperMock = new Mock<JWTTokenHelper>("aVeryLongSecureKeyThatIs256BitsLongUsedForHS256Signing", "AuthServiceIssuer", "AuthServiceAudience");
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _cache = new Mock<IDistributedCache>();

            _authService = new AuthServices(
                _userServiceMock.Object,
                _authRepositoryMock.Object,
                _refreshTokenRepositoryMock.Object,
                _jwtTokenHelperMock.Object,
                _httpContextAccessorMock.Object,
                _cache.Object
            );
        }

        // Helper method to mock the HttpContext and HttpResponse for cookies
        protected Mock<HttpResponse> SetupHttpContextForCookies()
        {
            var httpContextMock = new Mock<HttpContext>();
            var responseMock = new Mock<HttpResponse>();
            httpContextMock.Setup(ctx => ctx.Response).Returns(responseMock.Object);
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContextMock.Object);
            responseMock.Setup(r => r.Cookies.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>())).Verifiable();

            return responseMock;
        }
    }
}
