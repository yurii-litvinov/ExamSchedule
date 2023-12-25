// <copyright file="Lecturer.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

/// <summary>
/// Lecturer model.
/// </summary>
public class Lecturer : InputLecturer
{
    /// <summary>
    /// Gets or sets LecturerId column.
    /// </summary>
    public int LecturerId { get; set; }
}