// <copyright file="EmployeeQueriesTests.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamScheduleTests;

using ExamSchedule.Core;
using ExamSchedule.Core.Models;
using ExamSchedule.Core.Queries;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Employees queries tests.
/// </summary>
public class EmployeeQueriesTests
{
    private static readonly DbContextOptions<ScheduleContext> Options = new DbContextOptionsBuilder<ScheduleContext>()
        .UseInMemoryDatabase(databaseName: "postgres")
        .Options;

    private static readonly ScheduleContext DbContext = new(Options);
    private static readonly EmployeeQueries Queries = new(DbContext);

    private static readonly InputStaffWithoutRole InputEmployee = new()
    {
        Email = "test@test.ru",
        FirstName = "tester",
        LastName = "testov",
        MiddleName = "testevich",
        Password = "password",
    };

    /// <summary>
    /// Insert employee with empty password test.
    /// </summary>
    [Test]
    [Order(1)]
    public void InsertEmployeeWithEmptyPasswordTest()
    {
        var employeeWithoutPassword = new InputStaffWithoutRole()
        {
            Email = "test@test.ru",
            FirstName = "tester",
            LastName = "testov",
            MiddleName = "testevich",
            Password = string.Empty,
        };
        Assert.That(Queries.InsertEmployee(employeeWithoutPassword).Result, Is.InstanceOf<BadRequest<string>>());
    }

    /// <summary>
    /// Insert employee with empty email test.
    /// </summary>
    [Test]
    [Order(2)]
    public void InsertEmployeeWithEmptyEmailTest()
    {
        var employeeWithoutPassword = new InputStaffWithoutRole()
        {
            Email = string.Empty,
            FirstName = "tester",
            LastName = "testov",
            MiddleName = "testevich",
            Password = "password",
        };
        Assert.That(Queries.InsertEmployee(employeeWithoutPassword).Result, Is.InstanceOf<BadRequest<string>>());
    }

    /// <summary>
    /// Insert employee test.
    /// </summary>
    [Test]
    [Order(3)]
    public void InsertEmployeeTest()
    {
        Assert.DoesNotThrowAsync(() => Queries.InsertEmployee(InputEmployee));
    }

    /// <summary>
    /// Get employee test.
    /// </summary>
    [Test]
    [Order(4)]
    public void GetEmployeeTest()
    {
        var id = Queries.GetEmployees().Result.Last(employee => employee.Email == InputEmployee.Email).StaffId;
        var employee = Queries.GetEmployees(id).Result.First();
        Assert.Multiple(
            () =>
            {
                Assert.That(employee.FirstName, Is.EqualTo(InputEmployee.FirstName));
                Assert.That(employee.LastName, Is.EqualTo(InputEmployee.LastName));
                Assert.That(employee.MiddleName, Is.EqualTo(InputEmployee.MiddleName));
                Assert.That(employee.Email, Is.EqualTo(InputEmployee.Email));
                Assert.That(BCrypt.Net.BCrypt.Verify(InputEmployee.Password, employee.Password), Is.True);
            });
    }
}