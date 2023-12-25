// <copyright file="LocationQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using Npgsql;
using Dapper;
using Models;

/// <summary>
/// Location queries.
/// </summary>
public class LocationQueries : QueriesBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocationQueries"/> class.
    /// </summary>
    /// <param name="connection">Instance of <see cref="NpgsqlConnection"/>.</param>
    public LocationQueries(NpgsqlConnection connection)
        : base(connection)
    {
    }

    /// <summary>
    /// Gets locations.
    /// </summary>
    /// <param name="id">Location id, by default set to null.</param>
    /// <returns>List of locations.</returns>
    public async Task<IEnumerable<Location>> GetLocations(int? id = null)
    {
        string commandText = "select * from location";
        if (id != null)
        {
            commandText += $" where location_id = {id}";
        }

        var result = await this.connection.QueryAsync<Location>(commandText);
        return result.ToList();
    }

    /// <summary>
    /// Inserts new location.
    /// </summary>
    /// <param name="location">Input location.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> PostLocation(Location location)
    {
        await this.connection.QueryAsync<string>(
            $"insert into location(location_id, classroom) values ({location.LocationId}, '{location.Classroom}');");
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

            var classroom = string.IsNullOrEmpty(prev.Classroom) ? prev.Classroom : location.Classroom;
            await this.connection.QueryAsync<string>(
                $"update location set classroom = '{classroom}' where location_id = {id}");
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
        await this.connection.QueryAsync<string>(
            $"delete from location where location_id = {id};");
        return Results.Ok();
    }
}