using CourseService.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure JWT authentication for client requests
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
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
                        Console.WriteLine($"Token validated: {tokenId}");
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

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
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
        Title = "CourseService API",
        Version = "v1",
        Description = "API documentation for the CourseService"
    });
});

// Register custom services
builder.Services.ConfigurePersistence(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CourseService API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAll");
app.MapControllers();

app.Run();