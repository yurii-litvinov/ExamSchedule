// <copyright file="Location.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

/// <summary>
/// Location model.
/// </summary>
public class Location : InputLocation
{
    /// <summary>
    /// Gets or sets LocationID column.
    /// </summary>
    public int LocationId { get; set; }
}