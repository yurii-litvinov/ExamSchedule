// <copyright file="LecturerQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace ExamSchedule.Core.Queries;

using ExamSchedule.Core.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Lecturer queries.
/// </summary>
public class LecturerQueries(ScheduleContext context)
{
    private const int LecturerRoleId = (int)EnumRoles.LecturerRole;

    /// <summary>
    /// Gets lecturers.
    /// </summary>
    /// <param name="id">Lecturer id, by default set to null.</param>
    /// <returns>List of lecturers.</returns>
    public async Task<IEnumerable<Staff>> GetLecturers(int? id = null)
    {
        var result = context.Staffs.Where(staff => staff.RoleId == LecturerRoleId);
        if (id != null)
        {
            result = result.Where(staff => staff.StaffId == id);
        }

        return await result.ToListAsync();
    }

    /// <summary>
    /// Insets new lecturer.
    /// </summary>
    /// <param name="inputLecturer">Input lecturer.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> InsertLecturer(InputStaffWithoutRole inputLecturer)
    {
        if (inputLecturer.Password == string.Empty || inputLecturer.Email == string.Empty)
        {
            return Results.BadRequest("Checksum and Email fields are required");
        }

        var checksum = BCrypt.Net.BCrypt.HashPassword(inputLecturer.Password);
        var newLecturer = new Staff()
        {
            FirstName = inputLecturer.FirstName,
            LastName = inputLecturer.LastName,
            MiddleName = inputLecturer.MiddleName,
            Email = inputLecturer.Email,
            Password = checksum,
            RoleId = LecturerRoleId,
        };
        context.Staffs.Add(newLecturer);
        await context.SaveChangesAsync();
        return Results.Ok(newLecturer.StaffId);
    }
}