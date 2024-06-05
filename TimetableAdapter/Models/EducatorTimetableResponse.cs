// <copyright file="EducatorTimetableResponse.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace TimetableAdapter.Models;

/// <summary>
/// Educator timetable response model.
/// </summary>
public class EducatorTimetableResponse
{
    /// <summary>
    /// Gets or sets educator events days.
    /// </summary>
    public EducatorEventsDay[] EducatorEventsDays { get; set; } = [];
}