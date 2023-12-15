// <copyright file="InputLocation.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

/// <summary>
/// Location input model.
/// </summary>
public class InputLocation
{
    /// <summary>
    /// Gets or sets Classroom column.
    /// </summary>
    public string Classroom { get; set; } = string.Empty;
}