// <copyright file="StudentQueriesTests.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
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

    private static readonly StudentGroup Group = new StudentGroup()
    {
        Oid = 12345,
        Title = "group2",
        Description = "the best group in the world 1.2.3",
    };

    private static readonly InputStudent InputStudent = new()
    {
        FirstName = "stud",
        LastName = "studov",
        MiddleName = "studevich",
        StudentGroup = Group.Title,
    };

    /// <summary>
    /// Insert student test.
    /// </summary>
    [Test]
    [Order(1)]
    public void InsertStudentTest()
    {
        DbContext.StudentsGroups.Add(Group);
        DbContext.SaveChanges();
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
                student.StudentGroupOid == Group.Oid).StudentId;
        var student = Queries.GetStudents(id).Result.First();
        Assert.Multiple(
            () =>
            {
                Assert.That(student.FirstName, Is.EqualTo(InputStudent.FirstName));
                Assert.That(student.LastName, Is.EqualTo(InputStudent.LastName));
                Assert.That(student.MiddleName, Is.EqualTo(InputStudent.MiddleName));
                Assert.That(student.StudentGroupOid, Is.EqualTo(Group.Oid));
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
                student.StudentGroupOid == Group.Oid).StudentId;

        var newStudent = new InputStudent()
        {
            FirstName = "Ivan",
            LastName = "Ivanov",
            MiddleName = "Ivanovich",
            StudentGroup = Group.Title,
        };
        Assert.DoesNotThrowAsync(() => Queries.UpdateStudent(id, newStudent));

        var student = Queries.GetStudents(id).Result.First();
        Assert.Multiple(
            () =>
            {
                Assert.That(student.FirstName, Is.EqualTo(newStudent.FirstName));
                Assert.That(student.LastName, Is.EqualTo(newStudent.LastName));
                Assert.That(student.MiddleName, Is.EqualTo(newStudent.MiddleName));
                Assert.That(student.StudentGroupOid, Is.EqualTo(Group.Oid));
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
                student.StudentGroupOid == Group.Oid).StudentId;

        Assert.DoesNotThrowAsync(() => Queries.DeleteStudent(id));

        var student = Queries.GetStudents(id).Result.FirstOrDefault();
        Assert.That(student, Is.EqualTo(null));
    }
}