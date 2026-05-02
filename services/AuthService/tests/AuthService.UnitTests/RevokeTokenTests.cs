using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace AuthService.UnitTests;

public class RevokeTokenTests : BaseTestClass
{
    public RevokeTokenTests() : base() { }

    [Fact]
    public async Task RevokeToken_Should_Throw_Exception_When_Token_Is_Malformed()
    {
        // Arrange
        var malformedToken = "malformed-token";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _authService.RevokeTokenAsync(malformedToken));

        exception.Message.Should().Be("Malformed token");
    }

    [Fact]
    public async Task IsTokenRevoked_Should_Return_True_When_Token_Is_Revoked()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InRlc3QudXNlckBtYWlsLmNvbSIsInVzZXJJZCI6InRlc3QtdXNlcklkIiwianRpIjoic29tZS1qdGkifQ.LP4-HzqF1ZBg_1_rMGozWPGnWzju2Tsz-_SkLMlhzdc";
        var tokenId = "some-jti";

        _cache
            .Setup(c => c.GetAsync(tokenId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes("revoked"));

        // Act
        var result = await _authService.IsTokenRevokedAsync(token);

        // Assert
        result.Should().BeTrue();
    }
}