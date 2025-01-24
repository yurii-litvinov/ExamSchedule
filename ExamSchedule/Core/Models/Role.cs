// <copyright file="Role.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Role model.
/// </summary>
[Table("role")]
public sealed class Role
{
    /// <summary>
    /// Gets or sets Role_id column.
    /// </summary>
    [Key]
    [Column("role_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int RoleId { get; set; }

    /// <summary>
    /// Gets or sets Title column.
    /// </summary>
    [MaxLength(100)]
    [Column("title")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets Staff relation.
    /// </summary>
    public ICollection<Staff> Staffs { get; set; } = new List<Staff>();
}