// <copyright file="ExamQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using Microsoft.EntityFrameworkCore;
using Models;

/// <summary>
/// Exam queries.
/// </summary>
public class ExamQueries(ScheduleContext context)
{
    /// <summary>
    /// Gets exams.
    /// </summary>
    /// <param name="id">Exam id, by default set to null.</param>
    /// <returns>List of exams.</returns>
    public async Task<IEnumerable<object>> GetExams(int? id = null)
    {
        var result = await (from exam in context.Exams
            select new
            {
                exam.ExamId,
                Student_initials = exam.Student.FirstName + ' ' + exam.Student.LastName,
                exam.Title,
                Student_group = exam.Student.StudentGroup,
                Type = exam.Type.Title,
                exam.DateTime,
                exam.Location.Classroom,
                Lecturers = context.ExamLecturers.Where(el => el.ExamId == exam.ExamId).Select(el => el.Lecturer)
                    .ToList(),
                exam.TypeId,
                exam.StudentId,
                exam.LocationId,
            }).ToListAsync();
        if (id != null)
        {
            result = result.Where(e => e.ExamId == id).ToList();
        }

        return result;
    }

    /// <summary>
    /// Inserts new exam.
    /// </summary>
    /// <param name="exam">Input exam.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> InsertExam(InputExam exam)
    {
        var lecturersIds = exam.LecturersInitials.Select(
                lecturerInitials =>
                    context.Lecturers
                        .First(l => l.LastName + " " + l.FirstName + " " + l.MiddleName == lecturerInitials)
                        .LecturerId)
            .ToList();

        var typeId = context.ExamTypes.First(et => et.Title == exam.Type).ExamTypeId;
        var studentId = context.Students.First(
            s =>
                s.LastName + " " + s.FirstName + " " + s.MiddleName == exam.StudentInitials &&
                s.StudentGroup == exam.StudentGroup).StudentId;
        var locationId = context.Locations.First(loc => loc.Classroom == exam.Classroom).LocationId;

        var newExam = new Exam()
        {
            Title = exam.Title,
            TypeId = typeId,
            StudentId = studentId,
            DateTime = exam.DateTime,
            LocationId = locationId,
        };
        context.Exams.Add(newExam);
        foreach (var lecturerId in lecturersIds)
        {
            context.ExamLecturers.Add(
                new ExamLecturer()
                {
                    Exam = newExam,
                    LecturerId = lecturerId,
                });
        }

        await context.SaveChangesAsync();

        return Results.Ok();
    }

    /// <summary>
    /// Updates exam.
    /// </summary>
    /// <param name="id">Exam id.</param>
    /// <param name="exam">Input exam.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> UpdateExam(int id, InputExam exam)
    {
        var prev = context.Exams.First(e => e.ExamId == id);
        prev.Title = string.IsNullOrEmpty(exam.Title) ? prev.Title : exam.Title;
        prev.DateTime = exam.DateTime == DateTime.MinValue ? prev.DateTime : exam.DateTime;

        if (!string.IsNullOrEmpty(exam.Type))
        {
            prev.TypeId = context.ExamTypes.First(et => et.Title == exam.Type).ExamTypeId;
        }

        if (!string.IsNullOrEmpty(exam.StudentInitials) && !string.IsNullOrEmpty(exam.StudentGroup))
        {
            prev.StudentId = context.Students.First(
                s =>
                    s.LastName + " " + s.FirstName + " " + s.MiddleName == exam.StudentInitials &&
                    s.StudentGroup == exam.StudentGroup).StudentId;
        }

        if (!string.IsNullOrEmpty(exam.Classroom))
        {
            prev.LocationId = context.Locations.First(loc => loc.Classroom == exam.Classroom).LocationId;
        }

        if (!exam.LecturersInitials.Any())
        {
            await context.SaveChangesAsync();
            return Results.Ok();
        }

        var lecturersIds = exam.LecturersInitials.Select(
                lecturerInitials =>
                    context.Lecturers
                        .First(l => l.LastName + " " + l.FirstName + " " + l.MiddleName == lecturerInitials)
                        .LecturerId)
            .ToList();

        var examLectures = context.ExamLecturers.Where(el => el.ExamId == id);
        foreach (var el in examLectures)
        {
            context.Remove(el);
        }

        foreach (var lecturerId in lecturersIds)
        {
            context.ExamLecturers.Add(
                new ExamLecturer()
                {
                    ExamId = id,
                    LecturerId = lecturerId,
                });
        }

        await context.SaveChangesAsync();

        return Results.Ok();
    }

    /// <summary>
    /// Deletes exam.
    /// </summary>
    /// <param name="examId">Exam id.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> DeleteExam(int examId)
    {
        var exam = context.Exams.First(exam => exam.ExamId == examId);
        var examLectures = context.ExamLecturers.Where(el => el.ExamId == examId);
        context.Exams.Remove(exam);
        foreach (var el in examLectures)
        {
            context.Remove(el);
        }

        await context.SaveChangesAsync();
        return Results.Ok();
    }
}