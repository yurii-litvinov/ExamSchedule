// <copyright file="Program.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

using System.Security.Claims;
using System.Text;
using ExamSchedule;
using ExamSchedule.Core;
using ExamSchedule.Core.Auth;
using ExamSchedule.Core.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Get JWT options from appsettings
var jwtOptions = builder.Configuration
    .GetSection("JwtOptions")
    .Get<JwtOptions>();
if (jwtOptions != null)
{
    builder.Services.AddSingleton(jwtOptions);
}

// Add authentication
builder.Services.AddAuthentication("Bearer").AddJwtBearer(
    options =>
    {
        // convert the string signing key to byte array
        byte[] signingKeyBytes = Encoding.UTF8
            .GetBytes(jwtOptions?.SigningKey ?? string.Empty);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions?.Issuer ?? string.Empty,
            ValidAudience = jwtOptions?.Audience ?? string.Empty,
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
        };
    });

// Add authorization with policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("OnlyEmployee", policy => policy.RequireClaim(ClaimTypes.Role, "Сотрудник", "Админ"))
    .AddPolicy("OnlyAdministrator", policy => policy.RequireClaim(ClaimTypes.Role, "Админ"));

// Current environment
var currentEnvironment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Default";

// Register db context
builder.Services.AddDbContext<ScheduleContext>(
    opt => opt.UseNpgsql(builder.Configuration.GetConnectionString(currentEnvironment)));

// Add services to the container.
builder.Services.AddControllers().AddDataAnnotationsLocalization();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(
        options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add Authorize button in Swagger
builder.Services.AddSwaggerGen(
    swaggerOptions =>
    {
        swaggerOptions.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer",
            });
        swaggerOptions.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            });
    });

builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
            "CorsPolicy",
            policyBuilder => policyBuilder
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed((_) => true)
                .AllowAnyHeader());
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");

app.MapPost(
    "api/login/",
    async (ScheduleContext context, LoginModel model) =>
    {
        var staff = context.Staffs.FirstOrDefault(staff => staff.Email == model.Email);

        if (staff == null)
        {
            return Results.Unauthorized();
        }

        if (!BCrypt.Net.BCrypt.Verify(model.Password, staff.Password))
        {
            return Results.Unauthorized();
        }

        var accessToken = AuthenticationService.GetAccessToken(jwtOptions, staff, context);
        var refreshToken = AuthenticationService.GenerateRefreshToken();

        staff.RefreshToken = refreshToken;
        staff.RefreshTokenExpiry = DateTime.UtcNow.AddSeconds(jwtOptions?.RefreshExpirationSeconds ?? 0);

        await context.SaveChangesAsync();

        var result = new
        {
            accessToken,
            refreshToken,
            staff,
        };
        return Results.Json(result);
    });

app.MapPost(
    "api/refresh/",
    (ScheduleContext context, RefreshModel model) =>
    {
        var emailFromExpiredToken =
            AuthenticationService.GetEmailClaimFromExpiredToken(jwtOptions, model.AccessToken)?.Value ?? string.Empty;
        var staff = context.Staffs.FirstOrDefault(
            staff => staff.RefreshToken == model.RefreshToken && staff.Email == emailFromExpiredToken);

        if (staff == null)
        {
            return Results.Unauthorized();
        }

        if (staff.RefreshTokenExpiry < DateTime.UtcNow)
        {
            return Results.Unauthorized();
        }

        var accessToken = AuthenticationService.GetAccessToken(jwtOptions, staff, context);

        var result = new RefreshModel()
        {
            RefreshToken = model.RefreshToken,
            AccessToken = accessToken,
        };
        return Results.Json(result);
    });

app.MapPut(
    "api/update_table",
    (IFormFile? formFile, IFormFile tableFile) =>
    {
        var task = new ScheduleParser.ScheduleParser(formFile?.OpenReadStream()).ParseToTable(
            tableFile.OpenReadStream());
        return task.Result;
    }).RequireAuthorization();

// Exam Endpoints
app.MapGroup("api/exams/").ExamGroup().WithTags("Exams").RequireAuthorization();

// Student Endpoints
app.MapGroup("api/students/").StudentGroup().WithTags("Students").RequireAuthorization();

// Employee Endpoints
app.MapGroup("api/employees/").EmployeeGroup().WithTags("Employees").RequireAuthorization();

// Lecturer Endpoints
app.MapGroup("api/lecturers/").LecturerGroup().WithTags("Lecturers").RequireAuthorization();

// Location Endpoints
app.MapGroup("api/locations/").LocationGroup().WithTags("Locations").RequireAuthorization();

// Location Endpoints
app.MapGroup("api/timetable/").TimetableGroup().WithTags("Timetable").RequireAuthorization();

// Location Endpoints
app.MapGroup("api/staffs/").StaffGroup().WithTags("Staffs").RequireAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();