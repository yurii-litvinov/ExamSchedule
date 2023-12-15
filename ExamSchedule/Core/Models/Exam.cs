// <copyright file="Exam.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

/// <summary>
/// Exam model.
/// </summary>
public class Exam : InputExam
{
    /// <summary>
    /// Gets or sets Exam_ID column.
    /// </summary>
    public int ExamId { get; set; }

    /// <summary>
    /// Gets or sets Type_ID column.
    /// </summary>
    public int TypeId { get; set; }

    /// <summary>
    /// Gets or sets Student_ID column.
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// Gets or sets Location_ID column.
    /// </summary>
    public int LocationId { get; set; }
}