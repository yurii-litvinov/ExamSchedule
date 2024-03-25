// <copyright file="Student.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Student model.
/// </summary>
[Table("student")]
public class Student : InputStudent
{
    /// <summary>
    /// Gets or sets Student_id column.
    /// </summary>
    [Key]
    [Column("student_id")]
    public int StudentId { get; set; }

    /// <summary>
    /// Gets or sets Exams relation.
    /// </summary>
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}