// <copyright file="EndpointGroups.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule;

using Core;
using Core.Models;
using Core.Queries;

/// <summary>
/// Endpoints groups.
/// </summary>
public static class EndpointGroups
{
    /// <summary>
    /// Exam endpoints.
    /// </summary>
    /// <param name="group"><inheritdoc cref="RouteGroupBuilder" /></param>
    /// <returns>Exam route group builder.</returns>
    public static RouteGroupBuilder ExamGroup(this RouteGroupBuilder group)
    {
        group.MapGet(
            "/",
            (ScheduleContext ctx) => new ExamQueries(ctx).GetExams().Result);
        group.MapGet(
            "/{examId:int}",
            (int examId, ScheduleContext ctx) => new ExamQueries(ctx).GetExams(examId).Result);
        group.MapPost(
            "/",
            (InputExam exam, ScheduleContext ctx) => new ExamQueries(ctx).InsertExam(exam).Result);
        group.MapPut(
            "/",
            (int examId, InputExam exam, ScheduleContext ctx) => new ExamQueries(ctx).UpdateExam(examId, exam).Result);
        group.MapDelete(
            "/{examId:int}",
            (int examId, ScheduleContext ctx) => new ExamQueries(ctx).DeleteExam(examId).Result);

        return group;
    }

    /// <summary>
    /// Student endpoints.
    /// </summary>
    /// <param name="group"><inheritdoc cref="RouteGroupBuilder" /></param>
    /// <returns>Student route group builder.</returns>
    public static RouteGroupBuilder StudentGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", (ScheduleContext ctx) => new StudentQueries(ctx).GetStudents().Result);
        group.MapGet(
            "/{studentId:int}",
            (int studentId, ScheduleContext ctx) => new StudentQueries(ctx).GetStudents(studentId).Result);
        group.MapPost(
            "/",
            (InputStudent student, ScheduleContext ctx) => new StudentQueries(ctx).InsertStudent(student));
        group.MapPut(
            "/",
            (int studentId, InputStudent student, ScheduleContext ctx) =>
                new StudentQueries(ctx).UpdateStudent(studentId, student).Result);
        group.MapDelete(
            "/{studentId:int}",
            (int studentId, ScheduleContext ctx) => new StudentQueries(ctx).DeleteStudent(studentId).Result);

        return group;
    }

    /// <summary>
    /// Employee endpoints.
    /// </summary>
    /// <param name="group"><inheritdoc cref="RouteGroupBuilder" /></param>
    /// <returns>Employee route group builder.</returns>
    public static RouteGroupBuilder EmployeeGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", (ScheduleContext ctx) => new EmployeeQueries(ctx).GetEmployees().Result);
        group.MapPost(
            "/",
            (InputEmployee employee, ScheduleContext ctx) => new EmployeeQueries(ctx).InsertEmployee(employee).Result);
        group.MapPut(
            "/",
            (int employeeId, InputEmployee employee, ScheduleContext ctx) =>
                new EmployeeQueries(ctx).UpdateEmployee(employeeId, employee).Result);
        group.MapDelete(
            "/{employeeId:int}",
            (int employeeId, ScheduleContext ctx) => new EmployeeQueries(ctx).DeleteEmployee(employeeId).Result);

        return group;
    }

    /// <summary>
    /// Lecturer endpoints.
    /// </summary>
    /// <param name="group"><inheritdoc cref="RouteGroupBuilder" /></param>
    /// <returns>Lecturer route group builder.</returns>
    public static RouteGroupBuilder LecturerGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", (ScheduleContext ctx) => new LecturerQueries(ctx).GetLecturers().Result);
        group.MapGet(
            "/{lecturerId:int}",
            (int lecturerId, ScheduleContext ctx) => new LecturerQueries(ctx).GetLecturers(lecturerId).Result);
        group.MapPost(
            "/",
            (InputLecturer lecturer, ScheduleContext ctx) => new LecturerQueries(ctx).InsertLecturer(lecturer).Result);
        group.MapPut(
            "/",
            (int lecturerId, InputLecturer lecturer, ScheduleContext ctx) =>
                new LecturerQueries(ctx).UpdateLecturer(lecturerId, lecturer).Result);
        group.MapDelete(
            "/{lecturerId:int}",
            (int lecturerId, ScheduleContext ctx) => new LecturerQueries(ctx).DeleteLecturer(lecturerId).Result);

        return group;
    }

    /// <summary>
    /// Location endpoints.
    /// </summary>
    /// <param name="group"><inheritdoc cref="RouteGroupBuilder" /></param>
    /// <returns>Location route group builder.</returns>
    public static RouteGroupBuilder LocationGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", (ScheduleContext ctx) => new LocationQueries(ctx).GetLocations().Result);
        group.MapPost(
            "/",
            (Location location, ScheduleContext ctx) => new LocationQueries(ctx).InsertLocation(location).Result);
        group.MapPut(
            "/",
            (int locationId, InputLocation location, ScheduleContext ctx) =>
                new LocationQueries(ctx).UpdateLocation(locationId, location).Result);
        group.MapDelete(
            "/{locationId:int}",
            (int locationId, ScheduleContext ctx) => new LocationQueries(ctx).DeleteLocation(locationId).Result);

        return group;
    }
}