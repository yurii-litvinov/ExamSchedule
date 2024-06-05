// <copyright file="DayStudyEvent.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace TimetableAdapter.Models;

/// <summary>
/// Day study event model.
/// </summary>
public class DayStudyEvent
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
    /// Gets or sets dates.
    /// </summary>
    public string[] Dates { get; set; } = [];

    /// <summary>
    /// Gets or sets contingent unit names.
    /// </summary>
    public ContingentUnitName[] ContingentUnitNames { get; set; } = [];

    /// <summary>
    /// Gets or sets educators display text.
    /// </summary>
    public string EducatorsDisplayText { get; set; } = string.Empty;
}