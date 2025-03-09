using Microsoft.OpenApi.Models;
using AuthService.Infrastructure.Extensions;  // Custom extensions for registering services
using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;  // Add this namespace for Encoding
using AuthService.Application.Helpers;
using AuthService.Infrastructure.Repositories;
using System.Net.Http.Headers;
using AuthService.Application.Configuration;
using AuthService.Application.Middleware;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var secretKey = builder.Configuration["JwtSettings:SecretKey"];
var issuer = builder.Configuration["JwtSettings:Issuer"];
var audience = builder.Configuration["JwtSettings:Audience"];

var userServiceBaseUrl = builder.Configuration["UserService:BaseUrl"];

// Register Auth0Settings configuration
builder.Services.Configure<Auth0Settings>(builder.Configuration.GetSection("Auth0Settings"));

// Register JWTTokenHelper
builder.Services.AddScoped<JWTTokenHelper>(provider =>
{
    // Ensure all required settings are present
    if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
    {
        throw new ArgumentNullException("JWT settings are missing required parameters.");
    }

    return new JWTTokenHelper(secretKey, issuer, audience);
});

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Ensure all required settings are present
        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            throw new ArgumentNullException("JWT settings are missing required parameters.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddStackExchangeRedisCache(options =>
{
    // Get Redis connection string from environment variable
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    // Get Redis instance name from environment variable
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

// Add services to the container.
builder.Services.AddControllers();

// Register IHttpContextAccessor for dependency injection
builder.Services.AddHttpContextAccessor();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{AppDomain.CurrentDomain.FriendlyName}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AuthService API",
        Version = "v1",
        Description = "API documentation for the AuthService"
    });
});

builder.Services.ConfigurePersistence(builder.Configuration);  // Custom services

// Register IAuthService with its implementation (AuthService)
builder.Services.AddScoped<IAuthService, AuthServices>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});

// Register HttpClient for communication with the User Service
builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri(userServiceBaseUrl);  // This is where the User Service is hosted (configurable in appsettings.json)
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService API V1");
        c.RoutePrefix = string.Empty;  // Set Swagger UI at the root ("/")
    });
}

app.UseMiddleware<TokenRevocationMiddleware>();  // This ensures token revocation is checked
app.UseAuthentication();  // Authentication middleware
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
