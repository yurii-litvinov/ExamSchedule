// <copyright file="JwtOptions.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Auth;

/// <summary>
/// JWT options.
/// </summary>
/// <param name="Issuer">JWT issuer.</param>
/// <param name="Audience">JWT audience.</param>
/// <param name="SigningKey">JWT signing key.</param>
/// <param name="AccessExpirationSeconds">Access token expiration time in seconds.</param>
/// <param name="RefreshExpirationSeconds">Refresh token expiration time in seconds.</param>
public record JwtOptions(
    string Issuer,
    string Audience,
    string SigningKey,
    int AccessExpirationSeconds,
    int RefreshExpirationSeconds);