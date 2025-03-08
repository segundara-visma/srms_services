using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthService.Application.Helpers;

public class JWTTokenHelper
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpirationMinutes = 60; // 60minutes
    private readonly int _refreshTokenExpirationDays = 7; // 7 days for refresh token

    public JWTTokenHelper(string secretKey, string issuer, string audience)
    {
        _secretKey = secretKey;
        _issuer = issuer;
        _audience = audience;
    }

    // Generate Access Token
    public virtual string GenerateAccessToken(Guid userId, string userEmail)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Email, userEmail),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  // Add jti claim
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Generate Refresh Token
    public virtual string GenerateRefreshToken(Guid userId, string email)
    {
        var refreshToken = new JwtSecurityToken(
            _issuer,
            _audience,
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),  // User ID as NameIdentifier
                new Claim(ClaimTypes.Email, email),  // Add Email as a claim
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  // Add jti claim
            },
            expires: DateTime.UtcNow.AddDays(_refreshTokenExpirationDays),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(refreshToken);
    }

    // Validate Access Token
    public virtual ClaimsPrincipal ValidateAccessToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = key
        }, out var validatedToken);

        return principal;
    }

    public virtual ClaimsPrincipal ValidateRefreshToken(string refreshToken)
    {
        // Validate the refresh token and return ClaimsPrincipal
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(refreshToken) as JwtSecurityToken;

        if (jsonToken == null)
        {
            throw new InvalidOperationException("Invalid refresh token.");
        }

        // Create a ClaimsPrincipal from the claims in the token
        var claimsIdentity = new ClaimsIdentity(jsonToken?.Claims);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // You can now access the email and userId claims
        var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);
        if (emailClaim == null)
        {
            throw new InvalidOperationException("Refresh token does not contain an email claim.");
        }

        var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            throw new InvalidOperationException("Refresh token does not contain a user ID claim.");
        }

        // If you need to extract the values for userId and email, you can do it like this:
        var userId = Guid.Parse(userIdClaim.Value); // Assuming the userId is stored as a GUID in the token
        var email = emailClaim.Value; // Get the email from the token

        // Optionally, you can return both values as part of the ClaimsPrincipal
        claimsIdentity.AddClaim(new Claim("userId", userId.ToString()));
        claimsIdentity.AddClaim(new Claim("email", email));

        return claimsPrincipal;
    }
}
