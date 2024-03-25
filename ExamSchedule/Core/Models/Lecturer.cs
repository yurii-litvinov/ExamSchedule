// <copyright file="Lecturer.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Lecturer model.
/// </summary>
[Table("lecturer")]
public class Lecturer : InputLecturer
{
    /// <summary>
    /// Gets or sets Lecturer_id column.
    /// </summary>
    [Key]
    [Column("lecturer_id")]
    public int LecturerId { get; set; }
}