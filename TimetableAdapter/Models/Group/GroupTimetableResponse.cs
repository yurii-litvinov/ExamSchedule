// <copyright file="GroupTimetableResponse.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace TimetableAdapter.Models.Group;

/// <summary>
/// Group Timetable response model.
/// </summary>
public class GroupTimetableResponse
{
    /// <summary>
    /// Gets or sets group days.
    /// </summary>
    public Day[] Days { get; set; } = [];
}