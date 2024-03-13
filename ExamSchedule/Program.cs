// <copyright file="Program.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

using ExamSchedule;
using ExamSchedule.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSwaggerGen();

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
app.UseCors("CorsPolicy");

app.MapPut(
    "api/update_table",
    (string token, IFormFile? formFile, string filePath) =>
    {
        var task = new ScheduleParser.ScheduleParser(formFile?.OpenReadStream()).ParseToTable(token, filePath);
        return task.Result;
    });

// Exam Endpoints
app.MapGroup("api/exams/").ExamGroup().WithTags("Exams");

// Student Endpoints
app.MapGroup("api/students/").StudentGroup().WithTags("Students");

// Employee Endpoints
app.MapGroup("api/employees/").EmployeeGroup().WithTags("Employees");

// Lecturer Endpoints
app.MapGroup("api/lecturers/").LecturerGroup().WithTags("Lecturers");

// Location Endpoints
app.MapGroup("api/locations/").LocationGroup().WithTags("Locations");

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