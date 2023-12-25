// <copyright file="ExamQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using Npgsql;
using Dapper;
using Models;

/// <summary>
/// Exam queries.
/// </summary>
public class ExamQueries : QueriesBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExamQueries"/> class.
    /// </summary>
    /// <param name="connection">Instance of <see cref="NpgsqlConnection"/>.</param>
    public ExamQueries(NpgsqlConnection connection)
        : base(connection)
    {
    }

    /// <summary>
    /// Gets exams.
    /// </summary>
    /// <param name="id">Exam id, by default set to null.</param>
    /// <returns>List of exams.</returns>
    public async Task<IEnumerable<Exam>> GetExams(int? id = null)
    {
        string commandText = """
                             select exam.exam_id,
                                    (s.last_name || ' ' || s.first_name || ' ' || s.middle_name) as student_initials,
                                    exam.title,
                                    s.student_group,
                                    et.title                                                     as type,
                                    exam.date_time,
                                    loc.classroom,
                                    exam.type_id,
                                    exam.student_id,
                                    exam.location_id
                             from exam
                                      join public.student s on s.student_id = exam.student_id
                                      join public.exam_type et on et.exam_type_id = exam.type_id
                                      join public.location loc on exam.location_id = loc.location_id
                             """;
        if (id != null)
        {
            commandText += $" where exam.exam_id = {id}";
        }

        var result = (await this.connection.QueryAsync<Exam>(commandText)).ToList();
        foreach (var exam in result)
        {
            string examLecturerCommand = $"""
                                          select (l.last_name || ' ' || l.first_name || ' ' || l.middle_name) as lecturer_initials
                                          from exam_lecturer
                                                   join public.lecturer l on l.lecturer_id = exam_lecturer.lecturer_id
                                          where exam_id = {exam.ExamId};
                                          """;
            exam.LecturersInitials = (await this.connection.QueryAsync<string>(examLecturerCommand)).ToList();
        }

        return result;
    }

    /// <summary>
    /// Inserts new exam.
    /// </summary>
    /// <param name="exam">Input exam.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> PostExam(InputExam exam)
    {
        var lecturersIds = new List<string>();
        foreach (var lecturerInitials in exam.LecturersInitials)
        {
            lecturersIds.Add((await this.connection.QueryAsync<string>(
                $"""
                 select lecturer_id
                 from lecturer
                 where last_name || ' ' || first_name || ' ' || middle_name = '{lecturerInitials}';
                 """)).First());
        }

        var typeId =
            (await this.connection.QueryAsync<string>(
                $"select exam_type_id from exam_type where title = '{exam.Type}';"))
            .First();
        var studentId = (await this.connection.QueryAsync<string>(
            $"""
             select student_id
             from student
             where last_name || ' ' || first_name || ' ' || middle_name = '{exam.StudentInitials}'
               and student_group = '{exam.StudentGroup}';
             """)).First();
        var locationId = (await this.connection.QueryAsync<string>(
            $"select location_id from location where classroom = '{exam.Classroom}';")).First();
        var examId = (await this.connection.QueryAsync<int>("select * from nextval('exam_id_seq')")).First();

        string commandText =
            $"insert into Exam(Exam_ID, Title, Type_ID, Student_ID, Date_Time, Location_ID) " +
            $"values ({examId}, '{exam.Title}', {typeId}, {studentId}, '{exam.DateTime:yyyy-MM-dd hh:mm:ss}', {locationId});";
        await this.connection.QueryAsync<Exam>(commandText);
        foreach (var lecturerId in lecturersIds)
        {
            await this.connection.QueryAsync<string>(
                $"insert into Exam_Lecturer(Exam_ID, Lecturer_ID) VALUES ({examId}, {lecturerId});");
        }

        return Results.Ok();
    }

    /// <summary>
    /// Updates exam.
    /// </summary>
    /// <param name="id">Exam id.</param>
    /// <param name="exam">Input exam.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> UpdateExam(int id, InputExam exam)
    {
        var prev = this.GetExams(id).Result.First();
        var title = string.IsNullOrEmpty(exam.Title) ? prev.Title : exam.Title;
        var typeId = prev.TypeId;
        var studentId = prev.StudentId;
        var locationId = prev.LocationId;
        var dateTime = exam.DateTime == DateTime.MinValue ? prev.DateTime : exam.DateTime;

        if (!string.IsNullOrEmpty(exam.Type))
        {
            typeId =
                (await this.connection.QueryAsync<int>(
                    $"select exam_type_id from exam_type where title = '{prev.Type}';"))
                .First();
        }

        if (!string.IsNullOrEmpty(exam.StudentInitials) && !string.IsNullOrEmpty(exam.StudentGroup))
        {
            studentId = (await this.connection.QueryAsync<int>(
                $"""
                 select student_id
                 from student
                 where last_name || ' ' || first_name || ' ' || middle_name = '{prev.StudentInitials}'
                   and student_group = '{prev.StudentGroup}';
                 """)).First();
        }

        if (!string.IsNullOrEmpty(exam.Classroom))
        {
            locationId = (await this.connection.QueryAsync<int>(
                $"select location_id from location where classroom = '{prev.Classroom}';")).First();
        }

        string commandText =
            $"update exam set title = '{title}', type_id = {typeId}, student_id = {studentId}, " +
            $"location_id = {locationId}, date_time = '{dateTime:yyyy-MM-dd hh:mm:ss}' where exam_id = {id};";
        await this.connection.QueryAsync<Exam>(commandText);

        if (!exam.LecturersInitials.Any())
        {
            return Results.Ok();
        }

        var lecturersIds = new List<string>();
        foreach (var lecturerInitials in exam.LecturersInitials)
        {
            lecturersIds.Add((await this.connection.QueryAsync<string>(
                $"""
                 select lecturer_id
                 from lecturer
                 where last_name || ' ' || first_name || ' ' || middle_name = '{lecturerInitials}';
                 """)).First());
        }

        await this.connection.QueryAsync<string>(
            $"delete from exam_lecturer where exam_id = {id};");
        foreach (var lecturerId in lecturersIds)
        {
            await this.connection.QueryAsync<string>(
                $"insert into Exam_Lecturer(Exam_ID, Lecturer_ID) VALUES ({id}, {lecturerId});");
        }

        return Results.Ok();
    }

    /// <summary>
    /// Deletes exam.
    /// </summary>
    /// <param name="examId">Exam id.</param>
    /// <returns>Response status.</returns>
    public async Task<IResult> DeleteExam(int examId)
    {
        await this.connection.QueryAsync<Exam>($"delete from exam_lecturer where exam_id = {examId};");
        await this.connection.QueryAsync<Exam>($"delete from Exam where exam_id = {examId};");
        return Results.Ok();
    }
}