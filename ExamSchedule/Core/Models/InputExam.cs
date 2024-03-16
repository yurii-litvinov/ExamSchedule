// <copyright file="InputExam.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

/// <summary>
/// Exam input model.
/// </summary>
public class InputExam
{
    /// <summary>
    /// Gets or sets Title column.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets type of exam.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets student.
    /// </summary>
    public string StudentInitials { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets student group.
    /// </summary>
    public string StudentGroup { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets classroom.
    /// </summary>
    public string Classroom { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Date_Time column.
    /// </summary>
    public DateTime DateTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Gets or sets a value indicating whether exam is passed.
    /// </summary>
    public bool IsPassed { get; set; } = false;

    /// <summary>
    /// Gets or sets lecturer.
    /// </summary>
    public IEnumerable<string> LecturersInitials { get; set; } = Array.Empty<string>();
}