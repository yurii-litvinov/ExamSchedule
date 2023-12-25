// <copyright file="Employee.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

/// <summary>
/// Employee model.
/// </summary>
public class Employee : InputEmployee
{
    /// <summary>
    /// Gets or sets EmployeeId column.
    /// </summary>
    public int EmployeeId { get; set; }
}