// <copyright file="Educator.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace TimetableAdapter.Models;

/// <summary>
/// Educator model.
/// </summary>
public class Educator
{
    /// <summary>
    /// Gets or sets id.
    /// </summary>
    public int Id { get; set; } = 0;

    /// <summary>
    /// Gets or sets fullname.
    /// </summary>
    public string FullName { get; set; } = string.Empty;
}