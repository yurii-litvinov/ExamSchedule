// <copyright file="StudentQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using Microsoft.EntityFrameworkCore;
using Models;

/// <summary>
/// Student queries.
/// </summary>
public class StudentQueries(ScheduleContext context)
{
    /// <summary>
    /// Gets students.
    /// </summary>
    /// <param name="id">Student id, by default set to null.</param>
    /// <returns>List of students.</returns>
    public async Task<IEnumerable<Student>> GetStudents(int? id = null)
    {
        var result = await context.Students.ToListAsync();
        if (id != null)
        {
            result = result.Where(st => st.StudentId == id).ToList();
        }

        return result;
    }

    /// <summary>
    /// Inserts new student.
    /// </summary>
    /// <param name="st">Input student.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> InsertStudent(InputStudent st)
    {
        var student = new Student()
        {
            FirstName = st.FirstName,
            LastName = st.LastName,
            MiddleName = st.MiddleName,
            StudentGroup = st.StudentGroup,
        };
        context.Students.Add(student);
        await context.SaveChangesAsync();
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
            var prev = context.Students.First(s => s.StudentId == id);

            prev.StudentGroup = string.IsNullOrEmpty(st.StudentGroup) ? prev.StudentGroup : st.StudentGroup;
            prev.FirstName = string.IsNullOrEmpty(st.FirstName) ? prev.FirstName : st.FirstName;
            prev.LastName = string.IsNullOrEmpty(st.LastName) ? prev.LastName : st.LastName;
            prev.MiddleName = string.IsNullOrEmpty(st.MiddleName) ? prev.MiddleName : st.MiddleName;
            await context.SaveChangesAsync();
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
        var student = context.Students.First(st => st.StudentId == id);
        context.Students.Remove(student);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
}