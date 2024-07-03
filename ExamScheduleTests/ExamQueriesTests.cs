// <copyright file="ExamQueriesTests.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

// ReSharper disable SpecifyACultureInStringConversionExplicitly
namespace ExamScheduleTests;

using ExamSchedule.Core;
using ExamSchedule.Core.Models;
using ExamSchedule.Core.Queries;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Exam queries tests.
/// </summary>
public class ExamQueriesTests
{
    private static readonly DbContextOptions<ScheduleContext> Options = new DbContextOptionsBuilder<ScheduleContext>()
        .UseInMemoryDatabase(databaseName: "postgres")
        .Options;

    private static readonly ScheduleContext DbContext = new(Options);
    private static readonly ExamQueries Queries = new(DbContext);

    private static readonly InputStaffWithoutRole InputLecturer = new()
    {
        Email = "lect@lect.ru",
        FirstName = "lect",
        LastName = "lectov",
        MiddleName = "lectevich",
        Password = "lectorpass",
    };

    private static readonly InputStudent InputStudent = new()
    {
        FirstName = "stud",
        LastName = "studov",
        MiddleName = "studevich",
        StudentGroup = "22.Б22",
    };

    private static readonly InputLocation InputLocation = new()
    {
        Classroom = "2222",
    };

    private static readonly InputExam InputExam = new()
    {
        Title = "exam",
        Type = "Комиссия",
        StudentInitials = $"{InputStudent.LastName} {InputStudent.FirstName} {InputStudent.MiddleName}",
        StudentGroup = InputStudent.StudentGroup,
        Classroom = InputLocation.Classroom,
        LecturersInitials = new[] { $"{InputLecturer.LastName} {InputLecturer.FirstName} {InputLecturer.MiddleName}" },
    };

    /// <summary>
    /// Insert exam test.
    /// </summary>
    [Test]
    [Order(1)]
    public void InsertExamTest()
    {
        _ = new LecturerQueries(DbContext).InsertLecturer(InputLecturer).Result;
        _ = new StudentQueries(DbContext).InsertStudent(InputStudent).Result;
        _ = new LocationQueries(DbContext).InsertLocation(InputLocation).Result;
        DbContext.ExamTypes.Add(new ExamType() { Title = "Комиссия" });
        DbContext.SaveChanges();
        Assert.DoesNotThrowAsync(() => Queries.InsertExam(InputExam));
    }

    /// <summary>
    /// Get exam test.
    /// </summary>
    [Test]
    [Order(2)]
    public void GetExamTest()
    {
        var exam = Queries.GetExams().Result.Last();
        var id = (int)(exam.GetType().GetProperties().First(prop => prop.Name == "ExamId").GetValue(exam) ?? 0);
        var studentId = DbContext.Students.First(student => student.LastName == InputStudent.LastName).StudentId;
        var typeId = DbContext.ExamTypes.First(type => type.Title == InputExam.Type).ExamTypeId;
        var locationId = DbContext.Locations.First(location => location.Classroom == InputLocation.Classroom)
            .LocationId;
        var propsExpected = new Dictionary<string, string>()
        {
            { "Title", InputExam.Title },
            { "StudentId", studentId.ToString() },
            { "TypeId", typeId.ToString() },
            { "DateTime", InputExam.DateTime.ToString() },
            { "LocationId", locationId.ToString() },
        };
        var lecturerIds = InputExam.LecturersInitials.Select(
            lecturerInitials => DbContext.Staffs.First(
                staff => lecturerInitials == staff.LastName + " " + staff.FirstName + " " + staff.MiddleName).StaffId).ToList();
        var examLecturerIds = DbContext.ExamLecturers.Where(examLecturer => examLecturer.ExamId == id).Select(examLecturer => examLecturer.LecturerId).ToList();
        Assert.Multiple(
            () =>
            {
                foreach (var prop in exam.GetType().GetProperties())
                {
                    if (propsExpected.TryGetValue(prop.Name, out var value))
                    {
                        Assert.That(prop.GetValue(exam)?.ToString(), Is.EqualTo(value));
                    }
                }

                for (int i = 0; i < lecturerIds.Count(); i++)
                {
                    Assert.That(examLecturerIds[i], Is.EqualTo(lecturerIds[i]));
                }
            });
    }

    /// <summary>
    /// Update exam test.
    /// </summary>
    [Test]
    [Order(3)]
    public void UpdateExamTest()
    {
        var exam = Queries.GetExams().Result.Last();
        var id = (int)(exam.GetType().GetProperties().First(prop => prop.Name == "ExamId").GetValue(exam) ?? 0);

        InputStaffWithoutRole newInputLecturer = new()
        {
            Email = "lect@lect.ru",
            FirstName = "asdf",
            LastName = "asdfov",
            MiddleName = "asdfevich",
            Password = "lectorpass",
        };

        InputStudent newInputStudent = new()
        {
            FirstName = "ivan",
            LastName = "ivanov",
            MiddleName = "ivanevich",
            StudentGroup = "22.Б22",
        };

        InputLocation newInputLocation = new()
        {
            Classroom = "5555",
        };
        _ = new LecturerQueries(DbContext).InsertLecturer(newInputLecturer).Result;
        var studentId = new StudentQueries(DbContext).InsertStudent(newInputStudent).Result;
        var locationId = new LocationQueries(DbContext).InsertLocation(newInputLocation).Result;
        DbContext.ExamTypes.Add(new ExamType() { Title = "Пересдача" });
        DbContext.SaveChanges();
        var newInputExam = new InputExam()
        {
            Title = "d",
            Type = "Пересдача",
            StudentInitials = $"{newInputStudent.LastName} {newInputStudent.FirstName} {newInputStudent.MiddleName}",
            StudentGroup = newInputStudent.StudentGroup,
            Classroom = newInputLocation.Classroom,
            LecturersInitials = new[]
                {
                    // ReSharper disable once ArrangeTrailingCommaInSinglelineLists
                    $"{newInputLecturer.LastName} {newInputLecturer.FirstName} {newInputLecturer.MiddleName}",
                },
        };
        Assert.DoesNotThrowAsync(() => Queries.UpdateExam(id, newInputExam));
        var typeId = DbContext.ExamTypes.First(type => type.Title == newInputExam.Type).ExamTypeId;
        var propsExpected = new Dictionary<string, string>()
        {
            { "Title", newInputExam.Title },
            { "StudentId", studentId.ToString() },
            { "TypeId", typeId.ToString() },
            { "DateTime", newInputExam.DateTime.ToString() },
            { "LocationId", locationId.ToString() },
        };
        var lecturerIds = newInputExam.LecturersInitials.Select(
            lecturerInitials => DbContext.Staffs.First(
                staff => lecturerInitials == staff.LastName + " " + staff.FirstName + " " + staff.MiddleName).StaffId).ToList();
        var examLecturerIds = DbContext.ExamLecturers.Where(examLecturer => examLecturer.ExamId == id).Select(examLecturer => examLecturer.LecturerId).ToList();
        exam = Queries.GetExams().Result.Last();
        Assert.Multiple(
            () =>
            {
                foreach (var prop in exam.GetType().GetProperties())
                {
                    if (propsExpected.TryGetValue(prop.Name, out var value))
                    {
                        Assert.That(prop.GetValue(exam)?.ToString(), Is.EqualTo(value));
                    }
                }

                for (int i = 0; i < lecturerIds.Count(); i++)
                {
                    Assert.That(examLecturerIds[i], Is.EqualTo(lecturerIds[i]));
                }
            });
    }

    /// <summary>
    /// Delete exam test.
    /// </summary>
    [Test]
    [Order(4)]
    public void DeleteExamTest()
    {
        var exam = Queries.GetExams().Result.Last();
        var id = (int)(exam.GetType().GetProperties().First(prop => prop.Name == "ExamId").GetValue(exam) ?? 0);

        Assert.DoesNotThrowAsync(() => Queries.DeleteExam(id));

        var foundExam = Queries.GetExams(id).Result.FirstOrDefault();
        Assert.That(foundExam, Is.EqualTo(null));
    }
}