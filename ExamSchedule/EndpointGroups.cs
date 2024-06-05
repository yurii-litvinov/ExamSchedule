// <copyright file="EndpointGroups.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule;

// ReSharper disable RedundantNameQualifier
// ReSharper disable BadParensLineBreaks
using ExamSchedule.Core;
using ExamSchedule.Core.Models;
using ExamSchedule.Core.Queries;
using Microsoft.AspNetCore.Authorization;
using TimetableAdapter;

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
            [Authorize(Policy = "OnlyEmployee")]
            (InputExam exam, ScheduleContext context) =>
                new ExamQueries(context).InsertExam(exam).Result);
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
            [Authorize(Policy = "OnlyEmployee")]
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
            [Authorize(Policy = "OnlyAdministrator")]
            (InputStaffWithoutRole inputStaff, ScheduleContext context) =>
                new EmployeeQueries(context).InsertEmployee(inputStaff).Result);
        group.MapPut(
            "/",
            [Authorize(Policy = "OnlyAdministrator")]
            (int employeeId, InputStaff inputStaff, ScheduleContext context) =>
                new StaffQueries(context).UpdateStaff(employeeId, inputStaff).Result);
        group.MapDelete(
            "/{employeeId:int}",
            [Authorize(Policy = "OnlyAdministrator")]
            (int employeeId, ScheduleContext context) =>
                new StaffQueries(context).DeleteStaff(employeeId).Result);

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
            [Authorize(Policy = "OnlyAdministrator")]
            (InputStaffWithoutRole inputStaff, ScheduleContext context) =>
                new LecturerQueries(context).InsertLecturer(inputStaff).Result);
        group.MapPut(
            "/",
            [Authorize(Policy = "OnlyAdministrator")]
            (int lecturerId, InputStaff inputStaff, ScheduleContext context) =>
                new StaffQueries(context).UpdateStaff(lecturerId, inputStaff).Result);
        group.MapDelete(
            "/{lecturerId:int}",
            [Authorize(Policy = "OnlyAdministrator")]
            (int lecturerId, ScheduleContext context) =>
                new StaffQueries(context).DeleteStaff(lecturerId).Result);

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
            (InputLocation location, ScheduleContext context) =>
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

    /// <summary>
    /// Timetable endpoints.
    /// </summary>
    /// <param name="group"><inheritdoc cref="RouteGroupBuilder" /></param>
    /// <returns>Timetable route group builder.</returns>
    public static RouteGroupBuilder TimetableGroup(this RouteGroupBuilder group)
    {
        group.MapGet(
            "/educator/",
            (string lecturerInitials) => new TimetableQueries().GetEducatorsTimetable(lecturerInitials));
        group.MapGet(
            "/location/",
            (string location, string startDate, string endDate) =>
                new TimetableQueries().GetClassroomsTimetable(location, startDate, endDate));

        return group;
    }

    /// <summary>
    /// Staff endpoints.
    /// </summary>
    /// <param name="group"><inheritdoc cref="RouteGroupBuilder" /></param>
    /// <returns>Staff route group builder.</returns>
    public static RouteGroupBuilder StaffGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", (ScheduleContext context) => new StaffQueries(context).GetStaffs().Result);
        group.MapGet(
            "/{staffId:int}",
            (int staffId, ScheduleContext context) => new StaffQueries(context).GetStaffs(staffId).Result);
        group.MapGet(
            "/{staffId:int}/role",
            (int staffId, ScheduleContext context) => new StaffQueries(context).GetStaffRole(staffId));
        group.MapPut(
            "/",
            [Authorize(Policy = "OnlyAdministrator")]
            (int staffId, InputStaff inputStaff, ScheduleContext context) =>
                new StaffQueries(context).UpdateStaff(staffId, inputStaff).Result);
        group.MapDelete(
            "/{staffId:int}",
            [Authorize(Policy = "OnlyAdministrator")]
            (int staffId, ScheduleContext context) =>
                new StaffQueries(context).DeleteStaff(staffId).Result);

        return group;
    }
}