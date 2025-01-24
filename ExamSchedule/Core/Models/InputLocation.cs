// <copyright file="InputLocation.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Location input model.
/// </summary>
public class InputLocation
{
    /// <summary>
    /// Gets or sets Classroom column.
    /// </summary>
    [MaxLength(50)]
    [Column("classroom")]
    public string Classroom { get; set; } = string.Empty;
}