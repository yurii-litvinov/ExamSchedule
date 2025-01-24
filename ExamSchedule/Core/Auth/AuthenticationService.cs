// <copyright file="AuthenticationService.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace ExamSchedule.Core.Auth;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ExamSchedule.Core.Models;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Authentication service.
/// </summary>
public static class AuthenticationService
{
    /// <summary>
    /// Get access token.
    /// </summary>
    /// <param name="jwtOptions">JWT options.</param>
    /// <param name="staff">Access token owner.</param>
    /// <param name="context">Database context.</param>
    /// <returns>Returns access token.</returns>
    public static string GetAccessToken(JwtOptions? jwtOptions, Staff staff, ScheduleContext context)
    {
        var role = context.Roles.First(role => role.RoleId == staff.RoleId).Title;
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Email, staff.Email),
        };

        var keyBytes = Encoding.UTF8.GetBytes(jwtOptions?.SigningKey ?? string.Empty);
        var symmetricKey = new SymmetricSecurityKey(keyBytes);
        var signingCredentials = new SigningCredentials(
            symmetricKey,
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtOptions?.Issuer ?? string.Empty,
            audience: jwtOptions?.Audience ?? string.Empty,
            claims: claims,
            expires: DateTime.Now.Add(TimeSpan.FromSeconds(jwtOptions?.AccessExpirationSeconds ?? 0)),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generate refresh token.
    /// </summary>
    /// <returns>Returns refresh token.</returns>
    public static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];

        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Get email claim from expired access token.
    /// </summary>
    /// <param name="jwtOptions">JWT options.</param>
    /// <param name="token">Expired access token.</param>
    /// <returns>Returns email claim.</returns>
    public static Claim? GetEmailClaimFromExpiredToken(JwtOptions? jwtOptions, string? token)
    {
        byte[] signingKeyBytes = Encoding.UTF8
            .GetBytes(jwtOptions?.SigningKey ?? string.Empty);

        var validation = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
        };
        var principal = new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        return principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
    }
}