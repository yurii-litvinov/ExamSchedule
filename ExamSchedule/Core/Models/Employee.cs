// <copyright file="Employee.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Employee model.
/// </summary>
[Table("employee")]
public class Employee : InputEmployee
{
    /// <summary>
    /// Gets or sets Employee_id column.
    /// </summary>
    [Key]
    [Column("employee_id")]
    public int EmployeeId { get; set; }
}