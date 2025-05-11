using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace CourseService.Application.Services;

public class TokenValidationService
{
    private readonly IDistributedCache _cache;

    public TokenValidationService(IDistributedCache cache)
    {
        _cache = cache;
    }

    // Check if the token is revoked using tokenId (jti)
    public async Task<bool> IsTokenRevokedAsync(string tokenId)
    {
        if (tokenId == null)
        {
            Console.WriteLine("Token ID (jti) is missing from the token.");
            throw new InvalidOperationException("Token ID (jti) missing from the token.");
        }

        Console.WriteLine($"Checking revocation status for token ID: {tokenId}");

        // Check if the tokenId exists in the cache (i.e., if the token is revoked)
        var revokedToken = await _cache.GetStringAsync(tokenId);
        Console.WriteLine($"=============TokenValidation returns: ", revokedToken);
        return revokedToken != null;
    }
}
