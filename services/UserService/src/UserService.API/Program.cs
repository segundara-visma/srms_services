using Microsoft.OpenApi.Models;
using UserService.Infrastructure.Extensions;  // Custom extensions for registering services
using UserService.Application.Interfaces;
using UserService.Application.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;  // Add this namespace for Encoding
using UserService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure JWT authentication for User Client Requests
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Disable automatic claim type mapping
    // so as to avoid "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" for RoleClaimType
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

    // Optional: Enable logging for token validation errors
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            // Log the incoming Authorization header to check if it's being passed correctly
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                // Extract the token part after "Bearer " prefix
                var tokenString = authorizationHeader.ToString().Substring("Bearer ".Length).Trim();

                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.ReadToken(tokenString) as JwtSecurityToken;

                    if (token != null)
                    {
                        var tokenId = token.Id;  // Get token ID (jti)
                        Console.WriteLine($"Token validated: {tokenId}");

                        // Call your service to check if the token is revoked
                        var tokenRevocationService = context.HttpContext.RequestServices.GetRequiredService<TokenValidationService>();
                        var isRevoked = await tokenRevocationService.IsTokenRevokedAsync(tokenId);
                        Console.WriteLine($"IsTokenRevoked: {isRevoked}");

                        if (isRevoked)
                        {
                            context.Fail("Token is revoked.");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse JWT token.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing token: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("No Authorization header found.");
            }
        },
        OnAuthenticationFailed = context =>
        {
            // Suppress logging if the request will be handled by another scheme (e.g., Auth0)
            if (context.Request.Path.StartsWithSegments("/api/s2s"))
            {
                context.NoResult(); // Skip logging for S2S endpoints
                return Task.CompletedTask;
            }

            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        }
    };
})
// Add Auth0 authentication for Auth Service requests
.AddJwtBearer("Auth0", options =>
{
    options.Authority = builder.Configuration["Auth0:Authority"]; // Auth0 authorization server URL
    options.Audience = builder.Configuration["Auth0:Audience"];  // Auth0 API audience
    options.RequireHttpsMetadata = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Auth0:Authority"], // Auth0 Issuer URL
        ValidAudience = builder.Configuration["Auth0:Audience"]  // Auth0 API Audience
    };

    // Optional: Enable logging for Auth0 authentication errors
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Auth0 Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    // Get Redis connection string from environment variable
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    // Get Redis instance name from environment variable
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("TutorOnly", policy => policy.RequireRole("Tutor"));
    options.AddPolicy("AdminOrTutor", policy => policy.RequireRole("Admin", "Tutor"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{AppDomain.CurrentDomain.FriendlyName}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UserService API",
        Version = "v1",
        Description = "API documentation for the UserService"
    });
});

// Register your custom services
builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.AddScoped<IUserService, UserServices>();
builder.Services.AddScoped<TokenValidationService>();  // Ensure TokenValidationService is registered

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService API V1");
        c.RoutePrefix = string.Empty;  // Set Swagger UI at the root ("/")
    });
}

// Enable Authentication and Authorization Middleware
app.UseAuthentication();  // This will use the JWT Bearer Authentication middleware
app.UseAuthorization();

app.UseCors("AllowAll");
app.MapControllers();

app.Run();
