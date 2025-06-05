using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using EnrollmentService.Application.Configuration;
using EnrollmentService.Application.Interfaces;
using EnrollmentService.Application.Services;
using EnrollmentService.Infrastructure.Clients;
using EnrollmentService.Infrastructure.Persistence;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using EnrollmentService.Infrastructure.Extensions;
using EnrollmentService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure JWT authentication
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

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                var tokenString = authorizationHeader.ToString().Substring("Bearer ".Length).Trim();
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.ReadToken(tokenString) as JwtSecurityToken;
                    if (token != null)
                    {
                        var tokenId = token.Id;
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
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        }
    };
})
// Add Auth0 authentication for processing incoming requests from other services
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

// Configure Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

// Configure Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("TutorOnly", policy => policy.RequireRole("Tutor"));
    options.AddPolicy("AdminOrTutor", policy => policy.RequireRole("Admin", "Tutor"));
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{AppDomain.CurrentDomain.FriendlyName}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EnrollmentService API",
        Version = "v1",
        Description = "API documentation for the EnrollmentService"
    });
});

// Configure Persistence (assuming this is an extension method)
builder.Services.ConfigurePersistence(builder.Configuration);

builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentServiceImpl>();
builder.Services.AddScoped<TokenValidationService>();  // Ensure TokenValidationService is registered

// Configure Auth0 settings
builder.Services.Configure<Auth0Settings>(builder.Configuration.GetSection("Auth0Settings"));

// Register HttpClient with base addresses for UserService and CourseService
builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceSettings:UserServiceBaseUrl"] ?? "http://userservice:8080");
});
builder.Services.AddHttpClient<ICourseServiceClient, CourseServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceSettings:CourseServiceBaseUrl"] ?? "http://courseservice:8080");
});

// Configure Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EnrollmentService API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();