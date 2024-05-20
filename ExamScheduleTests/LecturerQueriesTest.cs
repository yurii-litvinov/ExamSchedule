// <copyright file="LecturerQueriesTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ExamScheduleTests;

using ExamSchedule.Core;
using ExamSchedule.Core.Models;
using ExamSchedule.Core.Queries;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Lecturer queries tests.
/// </summary>
public class LecturerQueriesTest
{
    private static readonly DbContextOptions<ScheduleContext> Options = new DbContextOptionsBuilder<ScheduleContext>()
        .UseInMemoryDatabase(databaseName: "postgres")
        .Options;

    private static readonly ScheduleContext DbContext = new(Options);
    private static readonly LecturerQueries Queries = new(DbContext);

    private static readonly InputStaffWithoutRole InputLecturer = new()
    {
        Email = "lect@lect.ru",
        FirstName = "lect",
        LastName = "lectov",
        MiddleName = "lectevich",
        Password = "lectorpass",
    };

    /// <summary>
    /// Insert lecturer test.
    /// </summary>
    [Test]
    [Order(1)]
    public void InsertLecturerTest()
    {
        Assert.DoesNotThrowAsync(() => Queries.InsertLecturer(InputLecturer));
    }

    /// <summary>
    /// Get lecturer test.
    /// </summary>
    [Test]
    [Order(2)]
    public void GetLecturerTest()
    {
        var id = Queries.GetLecturers().Result.Last(employee => employee.Email == InputLecturer.Email).StaffId;
        var employee = Queries.GetLecturers(id).Result.First();
        Assert.Multiple(
            () =>
            {
                Assert.That(employee.FirstName, Is.EqualTo(InputLecturer.FirstName));
                Assert.That(employee.LastName, Is.EqualTo(InputLecturer.LastName));
                Assert.That(employee.MiddleName, Is.EqualTo(InputLecturer.MiddleName));
                Assert.That(employee.Email, Is.EqualTo(InputLecturer.Email));
                Assert.That(BCrypt.Net.BCrypt.Verify(InputLecturer.Password, employee.Password), Is.True);
            });
    }
}