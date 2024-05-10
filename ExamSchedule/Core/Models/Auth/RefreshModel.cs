// <copyright file="RefreshModel.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Models.Auth;

/// <summary>
/// Refresh model.
/// </summary>
public class RefreshModel
{
    /// <summary>
    /// Gets or sets refresh token.
    /// </summary>
    public required string RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets access token.
    /// </summary>
    public required string AccessToken { get; set; }
}