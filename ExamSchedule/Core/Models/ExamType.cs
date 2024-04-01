// <copyright file="ExamType.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// ExamType model.
/// </summary>
[Table("exam_type")]
public sealed class ExamType
{
    /// <summary>
    /// Gets or sets ExamType_id column.
    /// </summary>
    [Key]
    [Column("exam_type_id")]
    public int ExamTypeId { get; set; }

    /// <summary>
    /// Gets or sets Title column.
    /// </summary>
    [MaxLength(100)]
    [Column("title")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets Exams relation.
    /// </summary>
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}