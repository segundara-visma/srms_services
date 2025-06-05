using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AdminService.Application.Configuration;
using AdminService.Application.Interfaces;
using AdminService.Application.Services;
using AdminService.Infrastructure.Clients;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

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
    options.MapInboundClaims = false; // Disable automatic claim type mapping
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
        RoleClaimType = "role" // "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
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
                        var tokenId = token.Id; // Assuming 'jti' claim
                        var tokenRevocationService = context.HttpContext.RequestServices.GetRequiredService<TokenValidationService>();
                        var isRevoked = await tokenRevocationService.IsTokenRevokedAsync(tokenId);
                        Console.WriteLine($"IsTokenRevoked: {isRevoked}");

                        if (isRevoked)
                        {
                            context.Fail("Token is revoked.");
                            return;
                        }

                        // Debug role claims
                        var roles = context.Principal?.Claims
                            .Where(c => c.Type == "role" || c.Type == options.TokenValidationParameters.RoleClaimType)
                            .Select(c => c.Value)
                            .ToList();
                        Console.WriteLine($"ClaimsPrincipal Roles: {string.Join(", ", roles ?? new List<string>())}");
                        Console.WriteLine($"All Claims: {string.Join(", ", context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}"))}");
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
        },
        OnChallenge = context =>
        {
            Console.WriteLine("Authentication challenge triggered: " + context.Error);
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
        Title = "AdminService API",
        Version = "v1",
        Description = "API documentation for the AdminService"
    });
});

//// Configure Persistence (assuming this is an extension method)
//builder.Services.ConfigurePersistence(builder.Configuration);

//builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminServiceImpl>();
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

// Register HttpClient with base addresses for UserService
builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceSettings:UserServiceBaseUrl"] ?? "http://userservice:8080");
});

// Register HttpClient with base addresses for TutorService
builder.Services.AddHttpClient<ITutorServiceClient, TutorServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceSettings:TutorServiceBaseUrl"] ?? "http://tutorservice:8080");
});

// Register HttpClient with base addresses for StudentService
builder.Services.AddHttpClient<IStudentServiceClient, StudentServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceSettings:StudentServiceBaseUrl"] ?? "http://studentservice:8080");
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AdminService API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();