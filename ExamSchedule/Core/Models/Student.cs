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
public class Student
{
    /// <summary>
    /// Gets or sets Student_id column.
    /// </summary>
    [Key]
    [Column("student_id")]
    public int StudentId { get; set; }

    /// <summary>
    /// Gets or sets FirstName column.
    /// </summary>
    [MaxLength(20)]
    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets LastName column.
    /// </summary>
    [MaxLength(20)]
    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets MiddleName column.
    /// </summary>
    [MaxLength(20)]
    [Column("middle_name")]
    public string MiddleName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets StudentGroup column.
    /// </summary>
    [Column("student_group")]
    public int StudentGroupOid { get; set; }

    /// <summary>
    /// Gets or sets Exams relation.
    /// </summary>
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}