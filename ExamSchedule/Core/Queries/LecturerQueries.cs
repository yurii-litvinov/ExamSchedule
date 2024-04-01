// <copyright file="LecturerQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using Microsoft.EntityFrameworkCore;
using Models;

/// <summary>
/// Lecturer queries.
/// </summary>
public class LecturerQueries(ScheduleContext context)
{
    /// <summary>
    /// Gets lecturers.
    /// </summary>
    /// <param name="id">Lecturer id, by default set to null.</param>
    /// <returns>List of lecturers.</returns>
    public async Task<IEnumerable<Lecturer>> GetLecturers(int? id = null)
    {
        var result = await context.Lecturers.ToListAsync();
        if (id != null)
        {
            result = result.Where(lec => lec.LecturerId == id).ToList();
        }

        return result;
    }

    /// <summary>
    /// Insets new lecturer.
    /// </summary>
    /// <param name="lec">Input lecturer.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> InsertLecturer(InputLecturer lec)
    {
        if (lec.Checksum == string.Empty || lec.Email == string.Empty)
        {
            return Results.BadRequest("Checksum and Email fields are required");
        }

        var newLecturer = new Lecturer()
        {
            FirstName = lec.FirstName,
            LastName = lec.LastName,
            MiddleName = lec.MiddleName,
            Email = lec.Email,
            Checksum = lec.Checksum,
        };
        context.Lecturers.Add(newLecturer);
        await context.SaveChangesAsync();
        return Results.Ok();
    }

    /// <summary>
    /// Updates lecturer.
    /// </summary>
    /// <param name="id">Lecturer id.</param>
    /// <param name="lec">Input lecturer.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> UpdateLecturer(int id, InputLecturer lec)
    {
        try
        {
            var prev = context.Lecturers.First(l => l.LecturerId == id);

            prev.Email = string.IsNullOrEmpty(lec.Email) ? prev.Email : lec.Email;
            prev.Checksum = string.IsNullOrEmpty(lec.Checksum) ? prev.Checksum : lec.Checksum;
            prev.FirstName = string.IsNullOrEmpty(lec.FirstName) ? prev.FirstName : lec.FirstName;
            prev.LastName = string.IsNullOrEmpty(lec.LastName) ? prev.LastName : lec.LastName;
            prev.MiddleName = string.IsNullOrEmpty(lec.MiddleName) ? prev.MiddleName : lec.MiddleName;
            await context.SaveChangesAsync();
            return Results.Ok();
        }
        catch (InvalidOperationException e)
        {
            return Results.BadRequest(e);
        }
    }

    /// <summary>
    /// Deletes lecturer.
    /// </summary>
    /// <param name="id">Lecturer id.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> DeleteLecturer(int id)
    {
        var lecturer = context.Lecturers.First(lec => lec.LecturerId == id);
        context.Lecturers.Remove(lecturer);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
}