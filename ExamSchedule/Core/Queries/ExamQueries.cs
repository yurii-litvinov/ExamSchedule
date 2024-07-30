// <copyright file="ExamQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

// ReSharper disable once RedundantNameQualifier
using ExamSchedule.Core.Models;
using Microsoft.EntityFrameworkCore;

// ReSharper disable RedundantNameQualifier
using ReportGenerator;

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
                StudentInitials = $"{exam.Student.LastName} {exam.Student.FirstName} {exam.Student.MiddleName}",
                exam.Title,
                exam.Student.StudentGroup,
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
                    context.Staffs
                        .First(
                            lecturer =>
                                (lecturer.LastName + " " + lecturer.FirstName + " " + lecturer.MiddleName).Trim() ==
                                lecturerInitials)
                        .StaffId)
            .ToList();

        var typeId = context.ExamTypes.First(examType => examType.Title == inputExam.Type).ExamTypeId;
        var studentId = context.Students.FirstOrDefault(
            student =>
                (student.LastName + " " + student.FirstName + " " + student.MiddleName).Trim() ==
                inputExam.StudentInitials &&
                student.StudentGroup == inputExam.StudentGroup)?.StudentId;
        if (studentId == null)
        {
            var initials = inputExam.StudentInitials.Split();
            if (initials.Length < 2)
            {
                return Results.BadRequest("Wrong student initials");
            }

            var middleName = initials.Length > 2 ? initials[2] : string.Empty;
            studentId = await new StudentQueries(context).InsertStudent(
                new InputStudent
                {
                    FirstName = initials[1],
                    LastName = initials[0],
                    MiddleName = middleName,
                    StudentGroup = inputExam.StudentGroup,
                });
        }

        var locationId = context.Locations.FirstOrDefault(location => location.Classroom == inputExam.Classroom)
            ?.LocationId;
        if (locationId == null)
        {
            locationId = await new LocationQueries(context).InsertLocation(
                new Location()
                {
                    Classroom = inputExam.Classroom,
                });
        }

        var newExam = new Exam()
        {
            Title = inputExam.Title,
            TypeId = typeId,
            StudentId = (int)studentId,
            DateTime = inputExam.DateTime,
            LocationId = (int)locationId,
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

        return Results.Ok(newExam.ExamId);
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
            var studentId = context.Students.FirstOrDefault(
                student =>
                    (student.LastName + " " + student.FirstName + " " + student.MiddleName).Trim() ==
                    inputExam.StudentInitials &&
                    student.StudentGroup == inputExam.StudentGroup)?.StudentId;
            if (studentId == null)
            {
                var initials = inputExam.StudentInitials.Split();
                if (initials.Length < 2)
                {
                    return Results.BadRequest("Wrong student initials");
                }

                var middleName = initials.Length > 2 ? initials[2] : string.Empty;
                studentId = await new StudentQueries(context).InsertStudent(
                    new InputStudent
                    {
                        FirstName = initials[1],
                        LastName = initials[0],
                        MiddleName = middleName,
                        StudentGroup = inputExam.StudentGroup,
                    });
            }

            prev.StudentId = (int)studentId;
        }

        if (!string.IsNullOrEmpty(inputExam.Classroom))
        {
            var locationId = context.Locations.FirstOrDefault(location => location.Classroom == inputExam.Classroom)
                ?.LocationId;
            if (locationId == null)
            {
                locationId = await new LocationQueries(context).InsertLocation(
                    new Location()
                    {
                        Classroom = inputExam.Classroom,
                    });
            }

            prev.LocationId = (int)locationId;
        }

        if (!inputExam.LecturersInitials.Any())
        {
            await context.SaveChangesAsync();
            return Results.Ok();
        }

        var lecturersIds = inputExam.LecturersInitials.Select(
                lecturerInitials =>
                    context.Staffs
                        .First(
                            lecturer =>
                                (lecturer.LastName + " " + lecturer.FirstName + " " + lecturer.MiddleName).Trim() ==
                                lecturerInitials)
                        .StaffId)
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
        foreach (var examLecturer in examLectures)
        {
            context.Remove(examLecturer);
        }

        context.Exams.Remove(deletedExam);

        await context.SaveChangesAsync();
        return Results.Ok();
    }

    /// <summary>
    /// Gets exam dto.
    /// </summary>
    /// <param name="id">Exam id.</param>
    /// <returns>List of exams.</returns>
    public ExamDto? GetExamDto(int id)
    {
        var queryable = (from exam in context.Exams
            select new
            {
                exam.ExamId,
                StudentInitials = $"{exam.Student.LastName} {exam.Student.FirstName} {exam.Student.MiddleName}",
                exam.Title,
                exam.Student.StudentGroup,
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
            }).Where(exam => exam.ExamId == id);

        var result = queryable.Select(
            exam => new ExamDto()
            {
                DateTime = exam.DateTime,
                Lecturers = exam.Lecturers.Select(
                    lecturer =>
                        $"{lecturer.LastName} {lecturer.FirstName.FirstOrDefault()}. {lecturer.MiddleName.FirstOrDefault()}."),
                Location = exam.Classroom,
                StudentGroup = exam.StudentGroup,
                StudentInitials = exam.StudentInitials,
                Title = exam.Title,
                TypeTitle = exam.Type,
            }).FirstOrDefault();

        return result;
    }

    /// <summary>
    /// Get from exam ids exam dtos.
    /// </summary>
    /// <param name="examIds">Exam ids.</param>
    /// <returns>Returns exam dtos.</returns>
    public IEnumerable<ExamDto> GetFromIdsDtos(IEnumerable<int> examIds)
    {
        return examIds.Select(this.GetExamDto).OfType<ExamDto>().ToList();
    }
}