// <copyright file="InputStudent.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Student input model.
/// </summary>
public class InputStudent
{
    /// <summary>
    /// Gets or sets StudentGroup.
    /// </summary>
    public string StudentGroup { get; set; } = string.Empty;

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
}