// <copyright file="EmployeeQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using Dapper;
using Models;

/// <summary>
/// Employee queries.
/// </summary>
public class EmployeeQueries : DbConnector
{
    /// <summary>
    /// Gets employees.
    /// </summary>
    /// <param name="id">Employee id, by default set to null.</param>
    /// <returns>List of employees.</returns>
    public async Task<IEnumerable<Employee>> GetEmployees(int? id = null)
    {
        string commandText = "select * from employee";
        if (id != null)
        {
            commandText += $" where employee_id = {id}";
        }

        var result = await this.connection.QueryAsync<Employee>(commandText);
        return result.ToList();
    }

    /// <summary>
    /// Inserts new Employee.
    /// </summary>
    /// <param name="emp">Input employee.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> PostEmployee(InputEmployee emp)
    {
        if (emp.Checksum == string.Empty || emp.Email == string.Empty)
        {
            return Results.BadRequest("Checksum and Email fields are required");
        }

        await this.connection.QueryAsync<string>(
            $"insert into employee(first_name, last_name, middle_name, email, checksum) " +
            $"values ('{emp.FirstName}', '{emp.LastName}', '{emp.MiddleName}', '{emp.Email}', '{emp.Checksum}');");
        return Results.Ok();
    }

    /// <summary>
    /// Updates employee.
    /// </summary>
    /// <param name="id"> Employee id.</param>
    /// <param name="emp">Input employee.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> UpdateEmployee(int id, InputEmployee emp)
    {
        try
        {
            var prev = this.GetEmployees(id).Result.First();

            var email = string.IsNullOrEmpty(emp.Email) ? prev.Email : emp.Email;
            var checksum = string.IsNullOrEmpty(emp.Checksum) ? prev.Checksum : emp.Checksum;
            var firstName = string.IsNullOrEmpty(emp.FirstName) ? prev.FirstName : emp.FirstName;
            var lastName = string.IsNullOrEmpty(emp.LastName) ? prev.LastName : emp.LastName;
            var middleName = string.IsNullOrEmpty(emp.MiddleName) ? prev.MiddleName : emp.MiddleName;
            await this.connection.QueryAsync<string>(
                $"update employee set email = '{email}', checksum = '{checksum}', first_name = '{firstName}'," +
                $" last_name = '{lastName}', middle_name = '{middleName}' where employee_id = {id}");
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
        await this.connection.QueryAsync<string>(
            $"delete from employee where employee_id = {id};");
        return Results.Ok();
    }
}