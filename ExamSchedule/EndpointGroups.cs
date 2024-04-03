// <copyright file="EndpointGroups.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule;

using ExamSchedule.Core;
using ExamSchedule.Core.Models;
using ExamSchedule.Core.Queries;

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
            (ScheduleContext context) => new ExamQueries(context).GetExams().Result);
        group.MapGet(
            "/{examId:int}",
            (int examId, ScheduleContext context) => new ExamQueries(context).GetExams(examId).Result);
        group.MapPost(
            "/",
            (InputExam exam, ScheduleContext context) => new ExamQueries(context).InsertExam(exam).Result);
        group.MapPut(
            "/",
            (int examId, InputExam inputExam, ScheduleContext context) =>
                new ExamQueries(context).UpdateExam(examId, inputExam).Result);
        group.MapDelete(
            "/{examId:int}",
            (int examId, ScheduleContext context) => new ExamQueries(context).DeleteExam(examId).Result);

        return group;
    }

    /// <summary>
    /// Student endpoints.
    /// </summary>
    /// <param name="group"><inheritdoc cref="RouteGroupBuilder" /></param>
    /// <returns>Student route group builder.</returns>
    public static RouteGroupBuilder StudentGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", (ScheduleContext context) => new StudentQueries(context).GetStudents().Result);
        group.MapGet(
            "/{studentId:int}",
            (int studentId, ScheduleContext context) => new StudentQueries(context).GetStudents(studentId).Result);
        group.MapPost(
            "/",
            (InputStudent inputStudent, ScheduleContext context) =>
                new StudentQueries(context).InsertStudent(inputStudent));
        group.MapPut(
            "/",
            (int studentId, InputStudent inputStudent, ScheduleContext context) =>
                new StudentQueries(context).UpdateStudent(studentId, inputStudent).Result);
        group.MapDelete(
            "/{studentId:int}",
            (int studentId, ScheduleContext context) => new StudentQueries(context).DeleteStudent(studentId).Result);

        return group;
    }

    /// <summary>
    /// Employee endpoints.
    /// </summary>
    /// <param name="group"><inheritdoc cref="RouteGroupBuilder" /></param>
    /// <returns>Employee route group builder.</returns>
    public static RouteGroupBuilder EmployeeGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", (ScheduleContext context) => new EmployeeQueries(context).GetEmployees().Result);
        group.MapPost(
            "/",
            (InputEmployee inputEmployee, ScheduleContext context) =>
                new EmployeeQueries(context).InsertEmployee(inputEmployee).Result);
        group.MapPut(
            "/",
            (int employeeId, InputEmployee inputEmployee, ScheduleContext context) =>
                new EmployeeQueries(context).UpdateEmployee(employeeId, inputEmployee).Result);
        group.MapDelete(
            "/{employeeId:int}",
            (int employeeId, ScheduleContext context) =>
                new EmployeeQueries(context).DeleteEmployee(employeeId).Result);

        return group;
    }

    /// <summary>
    /// Lecturer endpoints.
    /// </summary>
    /// <param name="group"><inheritdoc cref="RouteGroupBuilder" /></param>
    /// <returns>Lecturer route group builder.</returns>
    public static RouteGroupBuilder LecturerGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", (ScheduleContext context) => new LecturerQueries(context).GetLecturers().Result);
        group.MapGet(
            "/{lecturerId:int}",
            (int lecturerId, ScheduleContext context) => new LecturerQueries(context).GetLecturers(lecturerId).Result);
        group.MapPost(
            "/",
            (InputLecturer inputLecturer, ScheduleContext context) =>
                new LecturerQueries(context).InsertLecturer(inputLecturer).Result);
        group.MapPut(
            "/",
            (int lecturerId, InputLecturer inputLecturer, ScheduleContext context) =>
                new LecturerQueries(context).UpdateLecturer(lecturerId, inputLecturer).Result);
        group.MapDelete(
            "/{lecturerId:int}",
            (int lecturerId, ScheduleContext context) =>
                new LecturerQueries(context).DeleteLecturer(lecturerId).Result);

        return group;
    }

    /// <summary>
    /// Location endpoints.
    /// </summary>
    /// <param name="group"><inheritdoc cref="RouteGroupBuilder" /></param>
    /// <returns>Location route group builder.</returns>
    public static RouteGroupBuilder LocationGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", (ScheduleContext context) => new LocationQueries(context).GetLocations().Result);
        group.MapPost(
            "/",
            (Location location, ScheduleContext context) =>
                new LocationQueries(context).InsertLocation(location).Result);
        group.MapPut(
            "/",
            (int locationId, InputLocation inputLocation, ScheduleContext context) =>
                new LocationQueries(context).UpdateLocation(locationId, inputLocation).Result);
        group.MapDelete(
            "/{locationId:int}",
            (int locationId, ScheduleContext context) =>
                new LocationQueries(context).DeleteLocation(locationId).Result);

        return group;
    }
}