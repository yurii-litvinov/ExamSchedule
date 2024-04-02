// <copyright file="ExamQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using ExamSchedule.Core.Models;
using Microsoft.EntityFrameworkCore;

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
        var result = from exam in context.Exams
            select new
            {
                exam.ExamId,
                Student_initials = $"{exam.Student.FirstName} {exam.Student.LastName}",
                exam.Title,
                Student_group = exam.Student.StudentGroup,
                Type = exam.Type.Title,
                exam.DateTime,
                exam.IsPassed,
                exam.Location.Classroom,
                Lecturers = context.ExamLecturers.Where(examLecturer => examLecturer.ExamId == exam.ExamId)
                    .Select(examLecturer => examLecturer.Lecturer)
                    .ToList(),
                exam.TypeId,
                exam.StudentId,
                exam.LocationId,
            };
        if (id != null)
        {
            result = result.Where(exam => exam.ExamId == id);
        }

        return await result.ToListAsync();
    }

    /// <summary>
    /// Inserts new exam.
    /// </summary>
    /// <param name="inputExam">Input exam.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> InsertExam(InputExam inputExam)
    {
        var lecturersIds = inputExam.LecturersInitials.Select(
                lecturerInitials =>
                    context.Lecturers
                        .First(
                            lecturer => lecturer.LastName + " " + lecturer.FirstName + " " + lecturer.MiddleName ==
                                        lecturerInitials)
                        .LecturerId)
            .ToList();

        var typeId = context.ExamTypes.First(examType => examType.Title == inputExam.Type).ExamTypeId;
        var studentId = context.Students.First(
            student =>
                student.LastName + " " + student.FirstName + " " + student.MiddleName == inputExam.StudentInitials &&
                student.StudentGroup == inputExam.StudentGroup).StudentId;
        var locationId = context.Locations.First(location => location.Classroom == inputExam.Classroom).LocationId;

        var newExam = new Exam()
        {
            Title = inputExam.Title,
            TypeId = typeId,
            StudentId = studentId,
            DateTime = inputExam.DateTime,
            LocationId = locationId,
            IsPassed = false,
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
    /// <param name="inputExam">Input exam.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> UpdateExam(int id, InputExam inputExam)
    {
        var prev = context.Exams.First(exam => exam.ExamId == id);
        prev.Title = string.IsNullOrEmpty(inputExam.Title) ? prev.Title : inputExam.Title;
        prev.DateTime = inputExam.DateTime == DateTime.MinValue ? prev.DateTime : inputExam.DateTime;
        prev.IsPassed = inputExam.IsPassed;

        if (!string.IsNullOrEmpty(inputExam.Type))
        {
            prev.TypeId = context.ExamTypes.First(examType => examType.Title == inputExam.Type).ExamTypeId;
        }

        if (!string.IsNullOrEmpty(inputExam.StudentInitials) && !string.IsNullOrEmpty(inputExam.StudentGroup))
        {
            prev.StudentId = context.Students.First(
                student =>
                    student.LastName + " " + student.FirstName + " " + student.MiddleName ==
                    inputExam.StudentInitials &&
                    student.StudentGroup == inputExam.StudentGroup).StudentId;
        }

        if (!string.IsNullOrEmpty(inputExam.Classroom))
        {
            prev.LocationId = context.Locations.First(location => location.Classroom == inputExam.Classroom).LocationId;
        }

        if (!inputExam.LecturersInitials.Any())
        {
            await context.SaveChangesAsync();
            return Results.Ok();
        }

        var lecturersIds = inputExam.LecturersInitials.Select(
                lecturerInitials =>
                    context.Lecturers
                        .First(
                            lecturer => lecturer.LastName + " " + lecturer.FirstName + " " + lecturer.MiddleName ==
                                        lecturerInitials)
                        .LecturerId)
            .ToList();

        var examLectures = context.ExamLecturers.Where(examLecturer => examLecturer.ExamId == id);
        foreach (var examLecturer in examLectures)
        {
            context.Remove(examLecturer);
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
        var deletedExam = context.Exams.First(exam => exam.ExamId == examId);
        var examLectures = context.ExamLecturers.Where(examLecturer => examLecturer.ExamId == examId);
        context.Exams.Remove(deletedExam);
        foreach (var examLecturer in examLectures)
        {
            context.Remove(examLecturer);
        }

        await context.SaveChangesAsync();
        return Results.Ok();
    }
}