// <copyright file="StaffQueriesTests.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamScheduleTests;

using ExamSchedule;
using ExamSchedule.Core;
using ExamSchedule.Core.Models;
using ExamSchedule.Core.Queries;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Staff queries tests.
/// </summary>
public class StaffQueriesTests
{
    private static readonly DbContextOptions<ScheduleContext> Options = new DbContextOptionsBuilder<ScheduleContext>()
        .UseInMemoryDatabase(databaseName: "postgres")
        .Options;

    private static readonly ScheduleContext DbContext = new(Options);
    private static readonly StaffQueries Queries = new(DbContext);

    private static readonly InputStaffWithoutRole InputEmployee = new()
    {
        Email = "test@test.ru",
        FirstName = "tester",
        LastName = "testov",
        MiddleName = "testevich",
        Password = "password",
    };

    /// <summary>
    /// Get staff test.
    /// </summary>
    [Test]
    [Order(1)]
    public void GetStaffTest()
    {
        _ = new EmployeeQueries(DbContext).InsertEmployee(InputEmployee).Result;
        var id = Queries.GetStaffs().Result.Last(staff => staff.Email == InputEmployee.Email).StaffId;
        var staff = Queries.GetStaffs(id).Result.First();
        Assert.Multiple(
            () =>
            {
                Assert.That(staff.FirstName, Is.EqualTo(InputEmployee.FirstName));
                Assert.That(staff.LastName, Is.EqualTo(InputEmployee.LastName));
                Assert.That(staff.MiddleName, Is.EqualTo(InputEmployee.MiddleName));
                Assert.That(staff.Email, Is.EqualTo(InputEmployee.Email));
                Assert.That(BCrypt.Net.BCrypt.Verify(InputEmployee.Password, staff.Password), Is.True);
            });
    }

    /// <summary>
    /// Update staff test.
    /// </summary>
    [Test]
    [Order(2)]
    public void UpdateStaffTest()
    {
        var id = Queries.GetStaffs().Result.Last(
            staff => staff.Email == InputEmployee.Email).StaffId;

        var newStaff = new InputStaff()
        {
            FirstName = "Ivan",
            LastName = "Ivanov",
            MiddleName = "Ivanovich",
            Email = "staff@mail.ru",
            RoleId = (int)EnumRoles.LecturerRole,
            Password = "staffpass",
        };
        Assert.DoesNotThrowAsync(() => Queries.UpdateStaff(id, newStaff));

        var staff = Queries.GetStaffs(id).Result.First();
        newStaff.Email = InputEmployee.Email;
        Assert.Multiple(
            () =>
            {
                Assert.That(staff.FirstName, Is.EqualTo(newStaff.FirstName));
                Assert.That(staff.LastName, Is.EqualTo(newStaff.LastName));
                Assert.That(staff.MiddleName, Is.EqualTo(newStaff.MiddleName));
                Assert.That(staff.RoleId, Is.EqualTo(newStaff.RoleId));
                Assert.That(BCrypt.Net.BCrypt.Verify(newStaff.Password, staff.Password));
                Assert.DoesNotThrowAsync(() => Queries.UpdateStaff(id, newStaff));
            });
    }

    /// <summary>
    /// Delete staff test.
    /// </summary>
    [Test]
    [Order(3)]
    public void DeleteStaffTest()
    {
        var id = Queries.GetStaffs().Result.Last(
            staff => staff.Email == InputEmployee.Email).StaffId;

        Assert.DoesNotThrowAsync(() => Queries.DeleteStaff(id));

        var staff = Queries.GetStaffs(id).Result.FirstOrDefault();
        Assert.That(staff, Is.EqualTo(null));
    }
}