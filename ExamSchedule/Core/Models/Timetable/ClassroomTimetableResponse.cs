// <copyright file="ClassroomTimetableResponse.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models.Timetable;

/// <summary>
/// Classroom timetable response model.
/// </summary>
public class ClassroomTimetableResponse
{
    /// <summary>
    /// Gets or sets classroom events days.
    /// </summary>
    public ClassroomEventsDay[] ClassroomEventsDays { get; set; } = [];
}