// <copyright file="ExamLecturer.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// ExamLecturer model.
/// </summary>
[Table("exam_lecturer")]
public class ExamLecturer
{
    /// <summary>
    /// Gets or sets Id column.
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets Exam_id column.
    /// </summary>
    [Column("exam_id")]
    public int ExamId { get; set; }

    /// <summary>
    /// Gets or sets Lecturer_id column.
    /// </summary>
    [Column("lecturer_id")]
    public int LecturerId { get; set; }

    /// <summary>
    /// Gets or sets Exam relation.
    /// </summary>
    public Exam Exam { get; set; } = null!;

    /// <summary>
    /// Gets or sets Lecturer relation.
    /// </summary>
    public Staff Lecturer { get; set; } = null!;
}