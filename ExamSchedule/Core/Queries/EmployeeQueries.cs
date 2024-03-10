// <copyright file="EmployeeQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using Microsoft.EntityFrameworkCore;
using Models;

/// <summary>
/// Employee queries.
/// </summary>
public class EmployeeQueries(ScheduleContext context)
{
    /// <summary>
    /// Gets employees.
    /// </summary>
    /// <param name="id">Employee id, by default set to null.</param>
    /// <returns>List of employees.</returns>
    public async Task<IEnumerable<Employee>> GetEmployees(int? id = null)
    {
        var result = await context.Employees.ToListAsync();
        if (id != null)
        {
            result = result.Where(e => e.EmployeeId == id).ToList();
        }

        return result;
    }

    /// <summary>
    /// Inserts new Employee.
    /// </summary>
    /// <param name="emp">Input employee.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> InsertEmployee(InputEmployee emp)
    {
        if (emp.Checksum == string.Empty || emp.Email == string.Empty)
        {
            return Results.BadRequest("Checksum and Email fields are required");
        }

        var newEmployee = new Employee()
        {
            FirstName = emp.FirstName,
            LastName = emp.LastName,
            MiddleName = emp.MiddleName,
            Email = emp.Email,
            Checksum = emp.Checksum,
        };
        context.Employees.Add(newEmployee);
        await context.SaveChangesAsync();
        return Results.Ok();
    }

    /// <summary>
    /// Update employee.
    /// </summary>
    /// <param name="id"> Employee id.</param>
    /// <param name="emp">Input employee.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> UpdateEmployee(int id, InputEmployee emp)
    {
        try
        {
            var prev = context.Employees.First(e => e.EmployeeId == id);

            prev.Email = string.IsNullOrEmpty(emp.Email) ? prev.Email : emp.Email;
            prev.Checksum = string.IsNullOrEmpty(emp.Checksum) ? prev.Checksum : emp.Checksum;
            prev.FirstName = string.IsNullOrEmpty(emp.FirstName) ? prev.FirstName : emp.FirstName;
            prev.LastName = string.IsNullOrEmpty(emp.LastName) ? prev.LastName : emp.LastName;
            prev.MiddleName = string.IsNullOrEmpty(emp.MiddleName) ? prev.MiddleName : emp.MiddleName;
            await context.SaveChangesAsync();
            return Results.Ok();
        }
        catch (InvalidOperationException e)
        {
            return Results.BadRequest(e);
        }
    }

    /// <summary>
    /// Deletes employee.
    /// </summary>
    /// <param name="id">Employee id.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> DeleteEmployee(int id)
    {
        var employee = context.Employees.First(e => e.EmployeeId == id);
        context.Employees.Remove(employee);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
}