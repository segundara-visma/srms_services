using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Services;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add Redis as distributed cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "ApiGateway_";
});

// Register TokenValidationService
builder.Services.AddSingleton<TokenValidationService>();

// Add Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

// Configure JWT authentication with custom token validation
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("JwtBearer", options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
        RoleClaimType = "role"
    };

    // Add custom token validation to check for revocation
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var tokenValidationService = context.HttpContext.RequestServices.GetRequiredService<TokenValidationService>();
            var token = context.SecurityToken as JwtSecurityToken;
            var jti = token?.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;

            if (string.IsNullOrEmpty(jti))
            {
                context.Fail("Token ID (jti) missing from the token.");
                return;
            }

            var isRevoked = await tokenValidationService.IsTokenRevokedAsync(jti);
            if (isRevoked)
            {
                context.Fail("Token has been revoked.");
            }
        }
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
await app.UseOcelot();

app.Run();