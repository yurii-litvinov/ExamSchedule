// <copyright file="AddressesResponse.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace TimetableAdapter.Models;

/// <summary>
/// Addresses response model.
/// </summary>
public class AddressesResponse
{
    /// <summary>
    /// Gets or sets address oid.
    /// </summary>
    public string Oid { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets address display name.
    /// </summary>
    public string DisplayName1 { get; set; } = string.Empty;
}