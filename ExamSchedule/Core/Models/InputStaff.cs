// <copyright file="InputStaff.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Employee input model.
/// </summary>
public class InputStaff : InputStaffWithoutRole
{
    /// <summary>
    /// Gets or sets Role_id column.
    /// </summary>
    [Column("role_id")]
    public int RoleId { get; set; }
}