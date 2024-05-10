// <copyright file="EducatorsResponse.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models.Timetable;

/// <summary>
/// Educator response model.
/// </summary>
public class EducatorsResponse
{
    /// <summary>
    /// Gets or sets educators.
    /// </summary>
    public Educator[] Educators { get; set; } = [];
}