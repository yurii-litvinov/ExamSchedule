// <copyright file="InputStudent.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

/// <summary>
/// Student input model.
/// </summary>
public class InputStudent
{
    /// <summary>
    /// Gets or sets StudentGroup column.
    /// </summary>
    public string StudentGroup { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets FirstName column.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets LastName column.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets MiddleName column.
    /// </summary>
    public string MiddleName { get; set; } = string.Empty;
}