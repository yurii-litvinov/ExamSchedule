// <copyright file="Day.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace TimetableAdapter.Models.Group;

/// <summary>
/// Group's day model.
/// </summary>
public class Day
{
    /// <summary>
    /// Gets or sets day as string.
    /// </summary>
    public string DayString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets day study events.
    /// </summary>
    public GroupDayStudyEvent[] DayStudyEvents { get; set; } = [];
}