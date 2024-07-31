// <copyright file="StudentGroup.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Student model.
/// </summary>
[Table("student_group")]
public class StudentGroup
{
    /// <summary>
    /// Gets or sets Oid column.
    /// </summary>
    [Key]
    [Column("oid")]
    public int Oid { get; set; }

    /// <summary>
    /// Gets or sets Title column.
    /// </summary>
    [Column("title")]
    [MaxLength(20)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Description column.
    /// </summary>
    [Column("description")]
    [MaxLength(150)]
    public string Description { get; set; } = string.Empty;
}