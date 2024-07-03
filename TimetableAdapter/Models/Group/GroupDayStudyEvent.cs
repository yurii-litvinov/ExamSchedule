// <copyright file="GroupDayStudyEvent.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace TimetableAdapter.Models.Group;

/// <summary>
/// Group's day study event model.
/// </summary>
public class GroupDayStudyEvent
{
    /// <summary>
    /// Gets or sets subject.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets time interval as string.
    /// </summary>
    public string TimeIntervalString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets locations display text.
    /// </summary>
    public string LocationsDisplayText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets educators display text.
    /// </summary>
    public string EducatorsDisplayText { get; set; } = string.Empty;
}