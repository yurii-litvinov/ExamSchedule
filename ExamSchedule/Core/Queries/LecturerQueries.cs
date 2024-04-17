// <copyright file="LecturerQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using ExamSchedule.Core.Models;
using Microsoft.EntityFrameworkCore;

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
        var result = context.Lecturers.AsQueryable();
        if (id != null)
        {
            result = result.Where(lecturer => lecturer.LecturerId == id);
        }

        return await result.ToListAsync();
    }

    /// <summary>
    /// Insets new lecturer.
    /// </summary>
    /// <param name="inputLecturer">Input lecturer.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> InsertLecturer(InputLecturer inputLecturer)
    {
        if (inputLecturer.Checksum == string.Empty || inputLecturer.Email == string.Empty)
        {
            return Results.BadRequest("Checksum and Email fields are required");
        }

        var newLecturer = new Lecturer()
        {
            FirstName = inputLecturer.FirstName,
            LastName = inputLecturer.LastName,
            MiddleName = inputLecturer.MiddleName,
            Email = inputLecturer.Email,
            Checksum = inputLecturer.Checksum,
        };
        context.Lecturers.Add(newLecturer);
        await context.SaveChangesAsync();
        return Results.Ok(newLecturer.LecturerId);
    }

    /// <summary>
    /// Updates lecturer.
    /// </summary>
    /// <param name="id">Lecturer id.</param>
    /// <param name="inputLecturer">Input lecturer.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> UpdateLecturer(int id, InputLecturer inputLecturer)
    {
        try
        {
            var prev = context.Lecturers.First(lecturer => lecturer.LecturerId == id);

            prev.Email = string.IsNullOrEmpty(inputLecturer.Email) ? prev.Email : inputLecturer.Email;
            prev.Checksum = string.IsNullOrEmpty(inputLecturer.Checksum) ? prev.Checksum : inputLecturer.Checksum;
            prev.FirstName = string.IsNullOrEmpty(inputLecturer.FirstName) ? prev.FirstName : inputLecturer.FirstName;
            prev.LastName = string.IsNullOrEmpty(inputLecturer.LastName) ? prev.LastName : inputLecturer.LastName;
            prev.MiddleName = string.IsNullOrEmpty(inputLecturer.MiddleName)
                ? prev.MiddleName
                : inputLecturer.MiddleName;
            await context.SaveChangesAsync();
            return Results.Ok();
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(exception);
        }
    }

    /// <summary>
    /// Deletes lecturer.
    /// </summary>
    /// <param name="id">Lecturer id.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> DeleteLecturer(int id)
    {
        var deletedLecturer = context.Lecturers.First(lecturer => lecturer.LecturerId == id);
        context.Lecturers.Remove(deletedLecturer);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
}