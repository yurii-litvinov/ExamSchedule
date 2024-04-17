// <copyright file="EmployeeQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using ExamSchedule.Core.Models;
using Microsoft.EntityFrameworkCore;

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
        var result = context.Employees.AsQueryable();
        if (id != null)
        {
            result = result.Where(employee => employee.EmployeeId == id);
        }

        return await result.ToListAsync();
    }

    /// <summary>
    /// Inserts new Employee.
    /// </summary>
    /// <param name="employee">Input employee.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> InsertEmployee(InputEmployee employee)
    {
        if (employee.Checksum == string.Empty || employee.Email == string.Empty)
        {
            return Results.BadRequest("Checksum and Email fields are required");
        }

        var newEmployee = new Employee()
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            MiddleName = employee.MiddleName,
            Email = employee.Email,
            Checksum = employee.Checksum,
        };
        context.Employees.Add(newEmployee);
        await context.SaveChangesAsync();
        return Results.Ok(newEmployee.EmployeeId);
    }

    /// <summary>
    /// Update employee.
    /// </summary>
    /// <param name="id"> Employee id.</param>
    /// <param name="inputEmployee">Input employee.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> UpdateEmployee(int id, InputEmployee inputEmployee)
    {
        try
        {
            var prev = context.Employees.First(employee => employee.EmployeeId == id);

            prev.Email = string.IsNullOrEmpty(inputEmployee.Email) ? prev.Email : inputEmployee.Email;
            prev.Checksum = string.IsNullOrEmpty(inputEmployee.Checksum) ? prev.Checksum : inputEmployee.Checksum;
            prev.FirstName = string.IsNullOrEmpty(inputEmployee.FirstName) ? prev.FirstName : inputEmployee.FirstName;
            prev.LastName = string.IsNullOrEmpty(inputEmployee.LastName) ? prev.LastName : inputEmployee.LastName;
            prev.MiddleName = string.IsNullOrEmpty(inputEmployee.MiddleName)
                ? prev.MiddleName
                : inputEmployee.MiddleName;
            await context.SaveChangesAsync();
            return Results.Ok();
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(exception);
        }
    }

    /// <summary>
    /// Deletes employee.
    /// </summary>
    /// <param name="id">Employee id.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> DeleteEmployee(int id)
    {
        var deletedEmployee = context.Employees.First(employee => employee.EmployeeId == id);
        context.Employees.Remove(deletedEmployee);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
}