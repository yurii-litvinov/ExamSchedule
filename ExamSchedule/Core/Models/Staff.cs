// <copyright file="Staff.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Staff model.
/// </summary>
[Table("staff")]
public class Staff : InputStaff
{
    /// <summary>
    /// Gets or sets Staff_id column.
    /// </summary>
    [Key]
    [Column("staff_id")]
    public int StaffId { get; set; }

    /// <summary>
    /// Gets or sets Refresh_Token column.
    /// </summary>
    [Column("refresh_token")]
    [MaxLength(128)]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets Refresh_Token_Expiry column.
    /// </summary>
    [Column("refresh_token_expiry")]
    public DateTime? RefreshTokenExpiry { get; set; }
}