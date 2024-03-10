// <copyright file="LocationQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using Microsoft.EntityFrameworkCore;
using Models;

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
        var result = await context.Locations.ToListAsync();
        if (id != null)
        {
            result = result.Where(loc => loc.LocationId == id).ToList();
        }

        return result;
    }

    /// <summary>
    /// Inserts new location.
    /// </summary>
    /// <param name="location">Input location.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> InsertLocation(Location location)
    {
        context.Locations.Add(location);
        await context.SaveChangesAsync();
        return Results.Ok();
    }

    /// <summary>
    /// Updates location.
    /// </summary>
    /// <param name="id">Location id.</param>
    /// <param name="location">Input location.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> UpdateLocation(int id, InputLocation location)
    {
        try
        {
            var prev = this.GetLocations(id).Result.First();

            prev.Classroom = string.IsNullOrEmpty(prev.Classroom) ? prev.Classroom : location.Classroom;
            await context.SaveChangesAsync();
            return Results.Ok();
        }
        catch (InvalidOperationException e)
        {
            return Results.BadRequest(e);
        }
    }

    /// <summary>
    /// Deletes location.
    /// </summary>
    /// <param name="id">Location id.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> DeleteLocation(int id)
    {
        var location = context.Locations.First(loc => loc.LocationId == id);
        context.Locations.Remove(location);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
}