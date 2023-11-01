// <copyright file="SpreadsheetLoadResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ScheduleParser;

using Newtonsoft.Json;

/// <summary>
/// Class for deserializing <see cref="HttpContent"/>.
/// </summary>
public class SpreadsheetLoadResponse
{
    /// <summary>
    /// Gets or sets href value from response.
    /// </summary>
    [JsonProperty("href")]
    public string? Href { get; set; }

    /// <summary>
    /// Gets or sets method value from response.
    /// </summary>
    [JsonProperty("method")]
    public string? Method { get; set; }
}