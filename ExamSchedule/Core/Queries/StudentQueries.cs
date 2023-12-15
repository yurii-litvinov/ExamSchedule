// <copyright file="StudentQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using Dapper;
using Models;

/// <summary>
/// Student queries.
/// </summary>
public class StudentQueries : DbConnector
{
    /// <summary>
    /// Gets students.
    /// </summary>
    /// <param name="id">Student id, by default set to null.</param>
    /// <returns>List of students.</returns>
    public async Task<IEnumerable<Student>> GetStudents(int? id = null)
    {
        string commandText = "select * from student";
        if (id != null)
        {
            commandText += $" where student_id = {id}";
        }

        var result = await this.connection.QueryAsync<Student>(commandText);
        return result.ToList();
    }

    /// <summary>
    /// Inserts new student.
    /// </summary>
    /// <param name="st">Input student.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> PostStudent(InputStudent st)
    {
        await this.connection.QueryAsync<string>(
            $"insert into student(first_name, last_name, middle_name, student_group) " +
            $"VALUES ('{st.FirstName}', '{st.LastName}', '{st.MiddleName}', '{st.StudentGroup}');");
        return Results.Ok();
    }

    /// <summary>
    /// Updates student.
    /// </summary>
    /// <param name="id">Student id.</param>
    /// <param name="st">Input student.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> UpdateStudent(int id, InputStudent st)
    {
        try
        {
            var prev = this.GetStudents(id).Result.First();

            var group = string.IsNullOrEmpty(st.StudentGroup) ? prev.StudentGroup : st.StudentGroup;
            var firstName = string.IsNullOrEmpty(st.FirstName) ? prev.FirstName : st.FirstName;
            var lastName = string.IsNullOrEmpty(st.LastName) ? prev.LastName : st.LastName;
            var middleName = string.IsNullOrEmpty(st.MiddleName) ? prev.MiddleName : st.MiddleName;
            await this.connection.QueryAsync<string>(
                $"update student set student_group = '{group}', first_name = '{firstName}'," +
                $" last_name = '{lastName}', middle_name = '{middleName}' where student_id = {id}");
            return Results.Ok();
        }
        catch (InvalidOperationException e)
        {
            return Results.BadRequest(e);
        }
    }

    /// <summary>
    /// Deletes student.
    /// </summary>
    /// <param name="id">Student id.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> DeleteStudent(int id)
    {
        await this.connection.QueryAsync<string>(
            $"delete from student where student_id = {id};");
        return Results.Ok();
    }
}