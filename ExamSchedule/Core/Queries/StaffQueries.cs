// <copyright file="StaffQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace ExamSchedule.Core.Queries;

using ExamSchedule.Core.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Employee queries.
/// </summary>
public class StaffQueries(ScheduleContext context)
{
    /// <summary>
    /// Gets staff.
    /// </summary>
    /// <param name="id">Staff id.</param>
    /// <returns>Returns staff.</returns>
    public async Task<IEnumerable<Staff>> GetStaffs(int? id = null)
    {
        var result = context.Staffs.AsQueryable();
        if (id != null)
        {
            result = result.Where(staff => staff.StaffId == id);
        }

        return await result.ToListAsync();
    }

    /// <summary>
    /// Get staff role.
    /// </summary>
    /// <param name="id">Staff id.</param>
    /// <returns>Returns staff role.</returns>
    public string GetStaffRole(int id)
    {
        var staff = context.Staffs.First(staff => staff.StaffId == id);
        var role = context.Roles.First(role => role.RoleId == staff.RoleId).Title;

        return role;
    }

    /// <summary>
    /// Updates staff.
    /// </summary>
    /// <param name="id">Staff id.</param>
    /// <param name="inputStaff">Input staff.</param>
    /// <returns>Returns response status.</returns>
    public async Task<IResult> UpdateStaff(int id, InputStaff inputStaff)
    {
        try
        {
            var prev = context.Staffs.First(staff => staff.StaffId == id);

            prev.Email = string.IsNullOrEmpty(inputStaff.Email) ? prev.Email : inputStaff.Email;
            prev.Password = string.IsNullOrEmpty(inputStaff.Password)
                ? prev.Password
                : BCrypt.Net.BCrypt.HashPassword(inputStaff.Password);
            prev.FirstName = string.IsNullOrEmpty(inputStaff.FirstName) ? prev.FirstName : inputStaff.FirstName;
            prev.LastName = string.IsNullOrEmpty(inputStaff.LastName) ? prev.LastName : inputStaff.LastName;
            prev.MiddleName = string.IsNullOrEmpty(inputStaff.MiddleName)
                ? prev.MiddleName
                : inputStaff.MiddleName;
            await context.SaveChangesAsync();
            return Results.Ok();
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(exception);
        }
    }

    /// <summary>
    /// Deletes staff.
    /// </summary>
    /// <param name="id">Staff id.</param>
    /// <returns>Returns response status.</returns>
    public async Task<IResult> DeleteStaff(int id)
    {
        var deletedStaff = context.Staffs.First(staff => staff.StaffId == id);
        context.Staffs.Remove(deletedStaff);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
}