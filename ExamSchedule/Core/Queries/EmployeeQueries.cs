// <copyright file="EmployeeQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace ExamSchedule.Core.Queries;

using ExamSchedule.Core.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Employee queries.
/// </summary>
public class EmployeeQueries(ScheduleContext context)
{
    private const int EmployeeRoleId = 1;

    /// <summary>
    /// Gets employees.
    /// </summary>
    /// <param name="id">Employee id, by default set to null.</param>
    /// <returns>List of employees.</returns>
    public async Task<IEnumerable<Staff>> GetEmployees(int? id = null)
    {
        var result = context.Staffs.Where(staff => staff.RoleId == EmployeeRoleId);
        if (id != null)
        {
            result = result.Where(staff => staff.StaffId == id);
        }

        return await result.ToListAsync();
    }

    /// <summary>
    /// Inserts new Employee.
    /// </summary>
    /// <param name="inputEmployee">Input employee.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> InsertEmployee(InputStaffWithoutRole inputEmployee)
    {
        if (inputEmployee.Password == string.Empty || inputEmployee.Email == string.Empty)
        {
            return Results.BadRequest("Password and Email fields are required");
        }

        var checksum = BCrypt.Net.BCrypt.HashPassword(inputEmployee.Password);
        var newEmployee = new Staff()
        {
            FirstName = inputEmployee.FirstName,
            LastName = inputEmployee.LastName,
            MiddleName = inputEmployee.MiddleName,
            Email = inputEmployee.Email,
            Password = checksum,
            RoleId = EmployeeRoleId,
        };
        context.Staffs.Add(newEmployee);
        await context.SaveChangesAsync();
        return Results.Ok(newEmployee.StaffId);
    }
}