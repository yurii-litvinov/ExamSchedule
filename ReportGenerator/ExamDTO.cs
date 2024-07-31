// <copyright file="ExamDTO.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ReportGenerator;

/// <summary>
/// Exam DTO.
/// </summary>
public class ExamDto
{
    /// <summary>
    /// Gets or sets Title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Date_Time.
    /// </summary>
    public DateTime DateTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Gets or sets Location.
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets student initials.
    /// </summary>
    public string StudentInitials { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets student group.
    /// </summary>
    public string StudentGroup { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets student group description.
    /// </summary>
    public string StudentGroupDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets type.
    /// </summary>
    public string TypeTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets lecturers.
    /// </summary>
    public IEnumerable<string> Lecturers { get; set; } = [];
}