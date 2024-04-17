// <copyright file="StudentQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using ExamSchedule.Core.Models;
using Microsoft.EntityFrameworkCore;

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
        var result = context.Students.AsQueryable();
        if (id != null)
        {
            result = result.Where(student => student.StudentId == id);
        }

        return await result.ToListAsync();
    }

    /// <summary>
    /// Inserts new student.
    /// </summary>
    /// <param name="inputStudent">Input student.</param>
    /// <returns>Response status.</returns>
    public async Task<int> InsertStudent(InputStudent inputStudent)
    {
        var student = new Student()
        {
            FirstName = inputStudent.FirstName,
            LastName = inputStudent.LastName,
            MiddleName = inputStudent.MiddleName,
            StudentGroup = inputStudent.StudentGroup,
        };
        context.Students.Add(student);
        await context.SaveChangesAsync();
        return student.StudentId;
    }

    /// <summary>
    /// Updates student.
    /// </summary>
    /// <param name="id">Student id.</param>
    /// <param name="inputStudent">Input student.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> UpdateStudent(int id, InputStudent inputStudent)
    {
        try
        {
            var prev = context.Students.First(student => student.StudentId == id);

            prev.StudentGroup = string.IsNullOrEmpty(inputStudent.StudentGroup)
                ? prev.StudentGroup
                : inputStudent.StudentGroup;
            prev.FirstName = string.IsNullOrEmpty(inputStudent.FirstName) ? prev.FirstName : inputStudent.FirstName;
            prev.LastName = string.IsNullOrEmpty(inputStudent.LastName) ? prev.LastName : inputStudent.LastName;
            prev.MiddleName = string.IsNullOrEmpty(inputStudent.MiddleName) ? prev.MiddleName : inputStudent.MiddleName;
            await context.SaveChangesAsync();
            return Results.Ok();
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(exception);
        }
    }

    /// <summary>
    /// Deletes student.
    /// </summary>
    /// <param name="id">Student id.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> DeleteStudent(int id)
    {
        var deletedStudent = context.Students.First(student => student.StudentId == id);
        context.Students.Remove(deletedStudent);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
}