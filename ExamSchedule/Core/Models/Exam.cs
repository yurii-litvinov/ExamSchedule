// <copyright file="Exam.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Exam model.
/// </summary>
[Table("exam")]
public class Exam
{
    /// <summary>
    /// Gets or sets Exam_id column.
    /// </summary>
    [Key]
    [Column("exam_id")]
    public int ExamId { get; set; }

    /// <summary>
    /// Gets or sets Title column.
    /// </summary>
    [MaxLength(100)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Type_id column.
    /// </summary>
    [Column("type_id")]
    public int TypeId { get; set; }

    /// <summary>
    /// Gets or sets Student_id column.
    /// </summary>
    [Column("student_id")]
    public int StudentId { get; set; }

    /// <summary>
    /// Gets or sets Location_id column.
    /// </summary>
    [Column("location_id")]
    public int LocationId { get; set; }

    /// <summary>
    /// Gets or sets Date_Time column.
    /// </summary>
    [Column("date_time")]
    public DateTime DateTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Gets or sets Location relation.
    /// </summary>
    public Location Location { get; set; } = null!;

    /// <summary>
    /// Gets or sets Student relation.
    /// </summary>
    public Student Student { get; set; } = null!;

    /// <summary>
    /// Gets or sets ExamType relation.
    /// </summary>
    public ExamType Type { get; set; } = null!;
}