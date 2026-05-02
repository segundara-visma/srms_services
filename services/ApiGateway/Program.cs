using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ApiGateway.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

#region Configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile("ocelot.SwaggerEndPoints.json", optional: false, reloadOnChange: true);
#endregion

#region Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"]
        ?? throw new InvalidOperationException("Redis connection string is missing.");
    options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "ApiGateway_";
});
#endregion

#region Application Services
builder.Services.AddSingleton<TokenValidationService>();
builder.Services.AddHttpClient<EnrollmentAggregationService>();
builder.Services.AddHttpClient<TutorCoursesAggregationService>();
builder.Services.AddHttpClient<GradeAggregationService>();

builder.Services.AddControllers();
#endregion

#region Swagger & Ocelot Integration
builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SRMS API Gateway",
        Version = "v1",
        Description = "Unified API Gateway for Student Registration & Management System"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

#region Authentication (JWT)
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? string.Empty)),
            RoleClaimType = "role"
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var authHeader = context.HttpContext.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authHeader))
                {
                    var tokenString = authHeader.ToString().Substring("Bearer ".Length).Trim();
                    try
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var token = tokenHandler.ReadToken(tokenString) as JwtSecurityToken;

                        if (token != null)
                        {
                            var tokenId = token.Id;
                            var revocationService = context.HttpContext.RequestServices
                                .GetRequiredService<TokenValidationService>();

                            if (await revocationService.IsTokenRevokedAsync(tokenId))
                            {
                                context.Fail("Token is revoked.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Token validation error: {ex.Message}");
                    }
                }
            }
        };
    });
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
#endregion

#region Ocelot
builder.Services.AddOcelot(builder.Configuration);
#endregion

// ====================== Build App ======================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerForOcelotUI(
        ocelotOptions =>
        {
            ocelotOptions.PathToSwaggerGenerator = "/swagger/docs";
        },
        swaggerUiOptions =>
        {
            swaggerUiOptions.RoutePrefix = string.Empty;   // Swagger available at root: http://localhost:5000/
            swaggerUiOptions.DocumentTitle = "SRMS API Gateway - Unified Documentation";

            // Main merged document
            swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway Aggregation Endpoints API");
        });
}

app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map custom aggregation controllers
app.MapControllers();

// Ocelot handles all /api routes
app.MapWhen(context => context.Request.Path.StartsWithSegments("/api"),
    appBuilder => appBuilder.UseOcelot().Wait());

app.Run();