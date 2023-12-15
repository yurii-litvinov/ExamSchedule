// <copyright file="Student.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

/// <summary>
/// Student model.
/// </summary>
public class Student : InputStudent
{
    /// <summary>
    /// Gets or sets StudentId column.
    /// </summary>
    public int StudentId { get; set; }
}