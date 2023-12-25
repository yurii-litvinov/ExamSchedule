// <copyright file="Program.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule
{
    using ExamSchedule.Core;
    using ExamSchedule.Core.Models;
    using ExamSchedule.Core.Queries;

    /// <summary>
    /// Program class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Program entrypoint.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers().AddDataAnnotationsLocalization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
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
            var connector = new DbConnector();
            var connection = connector.Connection;
            app.UseCors("CorsPolicy");

            app.MapPut("api/update_table", (string token, IFormFile? formFile, string filePath) =>
            {
                var task = new ScheduleParser.ScheduleParser(formFile?.OpenReadStream()).ParseToTable(token, filePath);
                return task.Result;
            });

            // Exam Endpoints
            app.MapGet("api/exams/", () =>
            {
                var examQueries = new ExamQueries(connection);
                return examQueries.GetExams().Result;
            }).WithTags("Exams");
            app.MapGet("api/exams/{examId:int}", (int examId) =>
            {
                var examQueries = new ExamQueries(connection);
                return examQueries.GetExams(examId).Result;
            }).WithTags("Exams");
            app.MapPost("api/exams/", async (InputExam exam) =>
            {
                var examQueries = new ExamQueries(connection);
                return await examQueries.PostExam(exam);
            }).WithTags("Exams");
            app.MapPut("api/exams/", async (int examId, InputExam exam) =>
            {
                var examQueries = new ExamQueries(connection);
                return await examQueries.UpdateExam(examId, exam);
            }).WithTags("Exams");
            app.MapDelete("api/exams/{examId:int}", (int examId) =>
            {
                var examQueries = new ExamQueries(connection);
                return examQueries.DeleteExam(examId).Result;
            }).WithTags("Exams");

            // Student Endpoints
            app.MapGet("api/students/", () =>
            {
                var studentQueries = new StudentQueries(connection);
                return studentQueries.GetStudents().Result;
            }).WithTags("Students");
            app.MapGet("api/students/{studentId:int}", (int studentId) =>
            {
                var studentQueries = new StudentQueries(connection);
                return studentQueries.GetStudents(studentId).Result;
            }).WithTags("Students");
            app.MapPost("api/students/", (InputStudent student) =>
            {
                var studentQueries = new StudentQueries(connection);
                return studentQueries.PostStudent(student).Result;
            }).WithTags("Students");
            app.MapPut("api/students/", (int studentId, InputStudent student) =>
            {
                var studentQueries = new StudentQueries(connection);
                return studentQueries.UpdateStudent(studentId, student).Result;
            }).WithTags("Students");
            app.MapDelete("api/students/{studentId:int}", (int studentId) =>
            {
                var studentQueries = new StudentQueries(connection);
                return studentQueries.DeleteStudent(studentId).Result;
            }).WithTags("Students");

            // Employee Endpoints
            app.MapGet("api/employees/", () =>
            {
                var employeeQueries = new EmployeeQueries(connection);
                return employeeQueries.GetEmployees().Result;
            }).WithTags("Employees");
            app.MapPost("api/employees/", (InputEmployee employee) =>
            {
                var employeeQueries = new EmployeeQueries(connection);
                return employeeQueries.PostEmployee(employee).Result;
            }).WithTags("Employees");
            app.MapPut("api/employees/", (int employeeId, InputEmployee employee) =>
            {
                var employeeQueries = new EmployeeQueries(connection);
                return employeeQueries.UpdateEmployee(employeeId, employee).Result;
            }).WithTags("Employees");
            app.MapDelete("api/employees/{employeeId:int}", (int employeeId) =>
            {
                var employeeQueries = new EmployeeQueries(connection);
                return employeeQueries.DeleteEmployee(employeeId).Result;
            }).WithTags("Employees");

            // Lecturer Endpoints
            app.MapGet("api/lecturers/", () =>
            {
                var lecturerQueries = new LecturerQueries(connection);
                return lecturerQueries.GetLecturers().Result;
            }).WithTags("Lecturers");
            app.MapGet("api/lecturers/{lecturerId:int}", (int lecturerId) =>
            {
                var lecturerQueries = new LecturerQueries(connection);
                return lecturerQueries.GetLecturers(lecturerId).Result;
            }).WithTags("Lecturers");
            app.MapPost("api/lecturers/", (InputLecturer lecturer) =>
            {
                var lecturerQueries = new LecturerQueries(connection);
                return lecturerQueries.PostLecturer(lecturer).Result;
            }).WithTags("Lecturers");
            app.MapPut("api/lecturers/", (int lecturerId, InputLecturer lecturer) =>
            {
                var lecturerQueries = new LecturerQueries(connection);
                return lecturerQueries.UpdateLecturer(lecturerId, lecturer).Result;
            }).WithTags("Lecturers");
            app.MapDelete("api/lecturers/{lecturerId:int}", (int lecturerId) =>
            {
                var lecturerQueries = new LecturerQueries(connection);
                return lecturerQueries.DeleteLecturer(lecturerId).Result;
            }).WithTags("Lecturers");

            // Location Endpoints
            app.MapGet("api/locations/", () =>
            {
                var locationQueries = new LocationQueries(connection);
                return locationQueries.GetLocations().Result;
            }).WithTags("Locations");
            app.MapPost("api/locations/", (Location location) =>
            {
                var locationQueries = new LocationQueries(connection);
                return locationQueries.PostLocation(location).Result;
            }).WithTags("Locations");
            app.MapPut("api/locations/", (int locationId, InputLocation location) =>
            {
                var locationQueries = new LocationQueries(connection);
                return locationQueries.UpdateLocation(locationId, location).Result;
            }).WithTags("Locations");
            app.MapDelete("api/locations/{locationId:int}", (int locationId) =>
            {
                var locationQueries = new LocationQueries(connection);
                return locationQueries.DeleteLocation(locationId).Result;
            }).WithTags("Locations");

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
        }
    }
}