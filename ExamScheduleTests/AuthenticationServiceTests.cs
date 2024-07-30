// <copyright file="AuthenticationServiceTests.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamScheduleTests;

using ExamSchedule;
using ExamSchedule.Core;
using ExamSchedule.Core.Auth;
using ExamSchedule.Core.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Authentication service tests.
/// </summary>
public class AuthenticationServiceTests
{
    private static readonly DbContextOptions<ScheduleContext> Options = new DbContextOptionsBuilder<ScheduleContext>()
        .UseInMemoryDatabase(databaseName: "postgres")
        .Options;

    private static readonly ScheduleContext DbContext = new(Options);

    private readonly Staff staff = new Staff()
    {
        Email = "test@mail.ru",
        RoleId = (int)EnumRoles.AdminRole,
        Password = "12345qwerty",
    };

    private readonly JwtOptions jwtOptions = new JwtOptions(
        Issuer: "https://localhost:7107",
        Audience: "https://localhost:7107",
        SigningKey: "some-signing-key-here-very-long-must-be",
        AccessExpirationSeconds: 3600,
        RefreshExpirationSeconds: 86400);

    /// <summary>
    /// Setting up test.
    /// </summary>
    [Order(1)]
    [Test]
    public void SetUpTest()
    {
        Assert.DoesNotThrow(
            () =>
            {
                DbContext.Roles.Add(new Role() { RoleId = (int)EnumRoles.AdminRole, Title = "Админ" });
                DbContext.SaveChanges();
            });
    }

    /// <summary>
    /// Access tokens are not equal test.
    /// </summary>
    [Test]
    [Order(2)]
    public void AccessTokensAreNotEqualTest()
    {
        var firstToken = AuthenticationService.GetAccessToken(this.jwtOptions, this.staff, DbContext);
        Thread.Sleep(2000);
        var secondToken = AuthenticationService.GetAccessToken(this.jwtOptions, this.staff, DbContext);
        Assert.That(firstToken, Is.Not.EqualTo(secondToken));
    }

    /// <summary>
    /// Email claim is equal to staff.Email test.
    /// </summary>
    [Test]
    [Order(3)]
    public void GetEmailClaimFromTokenTest()
    {
        var firstToken = AuthenticationService.GetAccessToken(this.jwtOptions, this.staff, DbContext);
        var email = AuthenticationService.GetEmailClaimFromExpiredToken(this.jwtOptions, firstToken)?.Value ??
                    string.Empty;
        Assert.That(email, Is.EqualTo(this.staff.Email));
    }

    /// <summary>
    /// Refresh tokens are not equal test.
    /// </summary>
    [Test]
    [Order(4)]
    public void RefreshTokensAreNotEqualTest()
    {
        var firstToken = AuthenticationService.GenerateRefreshToken();
        var secondToken = AuthenticationService.GenerateRefreshToken();
        Assert.That(firstToken, Is.Not.EqualTo(secondToken));
    }
}