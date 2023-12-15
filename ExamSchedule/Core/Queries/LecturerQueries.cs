// <copyright file="LecturerQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using Dapper;
using Models;

/// <summary>
/// Lecturer queries.
/// </summary>
public class LecturerQueries : DbConnector
{
    /// <summary>
    /// Gets lecturers.
    /// </summary>
    /// <param name="id">Lecturer id, by default set to null.</param>
    /// <returns>List of lecturers.</returns>
    public async Task<IEnumerable<Lecturer>> GetLecturers(int? id = null)
    {
        string commandText = """select * from lecturer""";
        if (id != null)
        {
            commandText += $" where lecturer_id = {id}";
        }

        var result = await this.connection.QueryAsync<Lecturer>(commandText);
        return result.ToList();
    }

    /// <summary>
    /// Insets new lecturer.
    /// </summary>
    /// <param name="lec">Input lecturer.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> PostLecturer(InputLecturer lec)
    {
        if (lec.Checksum == string.Empty || lec.Email == string.Empty)
        {
            return Results.BadRequest("Checksum and Email fields are required");
        }

        await this.connection.QueryAsync<string>(
            $"insert into lecturer(first_name, last_name, middle_name, email, checksum) " +
            $"values ('{lec.FirstName}', '{lec.LastName}', '{lec.MiddleName}', '{lec.Email}', '{lec.Checksum}');");
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
            var prev = this.GetLecturers(id).Result.First();

            var email = string.IsNullOrEmpty(lec.Email) ? prev.Email : lec.Email;
            var checksum = string.IsNullOrEmpty(lec.Checksum) ? prev.Checksum : lec.Checksum;
            var firstName = string.IsNullOrEmpty(lec.FirstName) ? prev.FirstName : lec.FirstName;
            var lastName = string.IsNullOrEmpty(lec.LastName) ? prev.LastName : lec.LastName;
            var middleName = string.IsNullOrEmpty(lec.MiddleName) ? prev.MiddleName : lec.MiddleName;
            await this.connection.QueryAsync<string>(
                $"update lecturer set email = '{email}', checksum = '{checksum}', first_name = '{firstName}'," +
                $" last_name = '{lastName}', middle_name = '{middleName}' where lecturer_id = {id}");
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
        await this.connection.QueryAsync<string>(
            $"delete from lecturer where lecturer_id = {id};");
        return Results.Ok();
    }
}