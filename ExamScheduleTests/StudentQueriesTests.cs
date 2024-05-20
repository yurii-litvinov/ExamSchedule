// <copyright file="StudentQueriesTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ExamScheduleTests;

using ExamSchedule.Core;
using ExamSchedule.Core.Models;
using ExamSchedule.Core.Queries;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Student queries tests.
/// </summary>
public class StudentQueriesTests
{
    private static readonly DbContextOptions<ScheduleContext> Options = new DbContextOptionsBuilder<ScheduleContext>()
        .UseInMemoryDatabase(databaseName: "postgres")
        .Options;

    private static readonly ScheduleContext DbContext = new(Options);
    private static readonly StudentQueries Queries = new(DbContext);

    private static readonly Student InputStudent = new()
    {
        FirstName = "stud",
        LastName = "studov",
        MiddleName = "studevich",
        StudentGroup = "group",
    };

    /// <summary>
    /// Insert student test.
    /// </summary>
    [Test]
    [Order(1)]
    public void InsertStudentTest()
    {
        Assert.DoesNotThrowAsync(() => Queries.InsertStudent(InputStudent));
    }

    /// <summary>
    /// Get student test.
    /// </summary>
    [Test]
    [Order(2)]
    public void GetStudentTest()
    {
        var id = Queries.GetStudents().Result.Last(
            student =>
                student.LastName == InputStudent.LastName && student.FirstName == InputStudent.FirstName &&
                student.MiddleName == InputStudent.MiddleName &&
                student.StudentGroup == InputStudent.StudentGroup).StudentId;
        var student = Queries.GetStudents(id).Result.First();
        Assert.Multiple(
            () =>
            {
                Assert.That(student.FirstName, Is.EqualTo(InputStudent.FirstName));
                Assert.That(student.LastName, Is.EqualTo(InputStudent.LastName));
                Assert.That(student.MiddleName, Is.EqualTo(InputStudent.MiddleName));
                Assert.That(student.StudentGroup, Is.EqualTo(InputStudent.StudentGroup));
            });
    }

    /// <summary>
    /// Update student test.
    /// </summary>
    [Test]
    [Order(3)]
    public void UpdateStudentTest()
    {
        var id = Queries.GetStudents().Result.Last(
            student =>
                student.LastName == InputStudent.LastName && student.FirstName == InputStudent.FirstName &&
                student.MiddleName == InputStudent.MiddleName &&
                student.StudentGroup == InputStudent.StudentGroup).StudentId;

        var newStudent = new InputStudent()
        {
            FirstName = "Ivan",
            LastName = "Ivanov",
            MiddleName = "Ivanovich",
            StudentGroup = "22.Б22",
        };
        Assert.DoesNotThrowAsync(() => Queries.UpdateStudent(id, newStudent));

        var student = Queries.GetStudents(id).Result.First();
        Assert.Multiple(
            () =>
            {
                Assert.That(student.FirstName, Is.EqualTo(newStudent.FirstName));
                Assert.That(student.LastName, Is.EqualTo(newStudent.LastName));
                Assert.That(student.MiddleName, Is.EqualTo(newStudent.MiddleName));
                Assert.That(student.StudentGroup, Is.EqualTo(newStudent.StudentGroup));
                Assert.DoesNotThrowAsync(() => Queries.UpdateStudent(id, InputStudent));
            });
    }

    /// <summary>
    /// Delete student test.
    /// </summary>
    [Test]
    [Order(4)]
    public void DeleteStudentTest()
    {
        var id = Queries.GetStudents().Result.Last(
            student =>
                student.LastName == InputStudent.LastName && student.FirstName == InputStudent.FirstName &&
                student.MiddleName == InputStudent.MiddleName &&
                student.StudentGroup == InputStudent.StudentGroup).StudentId;

        Assert.DoesNotThrowAsync(() => Queries.DeleteStudent(id));

        var student = Queries.GetStudents(id).Result.FirstOrDefault();
        Assert.That(student, Is.EqualTo(null));
    }
}