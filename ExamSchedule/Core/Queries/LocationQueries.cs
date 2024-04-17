// <copyright file="LocationQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using ExamSchedule.Core.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Location queries.
/// </summary>
public class LocationQueries(ScheduleContext context)
{
    /// <summary>
    /// Gets locations.
    /// </summary>
    /// <param name="id">Location id, by default set to null.</param>
    /// <returns>List of locations.</returns>
    public async Task<IEnumerable<Location>> GetLocations(int? id = null)
    {
        var result = context.Locations.AsQueryable();
        if (id != null)
        {
            result = result.Where(location => location.LocationId == id);
        }

        return await result.ToListAsync();
    }

    /// <summary>
    /// Inserts new location.
    /// </summary>
    /// <param name="location">Input location.</param>
    /// <returns>Response status.</returns>
    public async Task<int> InsertLocation(Location location)
    {
        context.Locations.Add(location);
        await context.SaveChangesAsync();
        return location.LocationId;
    }

    /// <summary>
    /// Updates location.
    /// </summary>
    /// <param name="id">Location id.</param>
    /// <param name="inputLocation">Input location.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> UpdateLocation(int id, InputLocation inputLocation)
    {
        try
        {
            var prev = this.GetLocations(id).Result.First();

            prev.Classroom = string.IsNullOrEmpty(prev.Classroom) ? prev.Classroom : inputLocation.Classroom;
            await context.SaveChangesAsync();
            return Results.Ok();
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(exception);
        }
    }

    /// <summary>
    /// Deletes location.
    /// </summary>
    /// <param name="id">Location id.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> DeleteLocation(int id)
    {
        var deletedLocation = context.Locations.First(location => location.LocationId == id);
        context.Locations.Remove(deletedLocation);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
}