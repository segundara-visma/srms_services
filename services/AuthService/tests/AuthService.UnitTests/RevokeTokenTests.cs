using System;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FluentAssertions;
using System.Text;  // Add the System.Text namespace for Encoding

namespace AuthService.UnitTests
{
    public class RevokeTokenTests : BaseTestClass
    {
        public RevokeTokenTests() : base() {}

        [Fact]
        public async Task RevokeToken_Should_Throw_Exception_When_Token_Is_Malformed()
        {
            // Arrange
            var malformedToken = "malformed-token";  // Simulate a token without jti

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RevokeTokenAsync(malformedToken));
            Assert.Equal("Malformed token", exception.Message);  // Verify the exception message
        }

        [Fact]
        public async Task IsTokenRevoked_Should_Return_True_When_Token_Is_Revoked()
        {
            // Arrange
            //var token = "revoked-token";
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InRlc3QudXNlckBtYWlsLmNvbSIsInVzZXJJZCI6InRlc3QtdXNlcklkIiwianRpIjoic29tZS1qdGkifQ.LP4-HzqF1ZBg_1_rMGozWPGnWzju2Tsz-_SkLMlhzdc";
            var tokenId = "some-jti";
            var userId = Guid.NewGuid();
            var email = "test.user@mail.com";

            // Use CancellationToken.None to avoid optional argument issues
            _cache.Setup(c => c.GetAsync(tokenId, CancellationToken.None)).ReturnsAsync(Encoding.UTF8.GetBytes("revoked"));

            // Act
            var isRevoked = await _authService.IsTokenRevokedAsync(token);

            // Assert
            isRevoked.Should().BeTrue();  // Ensure FluentAssertions is being used
        }
    }
}
