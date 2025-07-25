using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReportService.Application.Configuration;
using ReportService.Application.Interfaces;
using ReportService.Application.Services;
using ReportService.Infrastructure.Clients;
using ReportService.Infrastructure.Persistence;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using ReportService.Infrastructure.Extensions;
using ReportService.Infrastructure.Repositories;

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
        Title = "ReportService API",
        Version = "v1",
        Description = "API documentation for the ReportService"
    });
});

// Configure Persistence (assuming this is an extension method)
builder.Services.ConfigurePersistence(builder.Configuration);

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportServiceImpl>();
builder.Services.AddScoped<TokenValidationService>();  // Ensure TokenValidationService is registered

// Configure Auth0 settings
builder.Services.Configure<Auth0Settings>(builder.Configuration.GetSection("Auth0Settings"));

// Register HttpClient with base addresses for EnrollmentService
builder.Services.AddHttpClient<IEnrollmentServiceClient, EnrollmentServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceSettings:EnrollmentServiceBaseUrl"] ?? "http://enrollmentservice:8080");
});

// Register HttpClient with base addresses for GradeService
builder.Services.AddHttpClient<IGradeServiceClient, GradeServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceSettings:GradeServiceBaseUrl"] ?? "http://gradeservice:8080");
});

// Register HttpClient with base addresses for CourseService
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReportService API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();