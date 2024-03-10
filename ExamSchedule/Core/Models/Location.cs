// <copyright file="Location.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Location model.
/// </summary>
[Table("location")]
public class Location : InputLocation
{
    /// <summary>
    /// Gets or sets Location_id column.
    /// </summary>
    [Key]
    [Column("location_id")]
    public int LocationId { get; set; }

    /// <summary>
    /// Gets or sets Exams relation.
    /// </summary>
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}