// <copyright file="InputEmployee.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Employee input model.
/// </summary>
public class InputEmployee
{
    /// <summary>
    /// Gets or sets Email column.
    /// </summary>
    [MaxLength(50)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Checksum column.
    /// </summary>
    [MaxLength(50)]
    [Column("checksum")]
    public string Checksum { get; set; } = string.Empty;

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