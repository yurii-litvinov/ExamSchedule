// <copyright file="ClassroomEventsDay.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models.Timetable;

/// <summary>
/// Classroom events day model.
/// </summary>
public class ClassroomEventsDay
{
    /// <summary>
    /// Gets or sets day as string.
    /// </summary>
    public string DayString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets day study events.
    /// </summary>
    public DayStudyEvent[] DayStudyEvents { get; set; } = [];
}