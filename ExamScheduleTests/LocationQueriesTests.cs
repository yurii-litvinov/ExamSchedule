// <copyright file="LocationQueriesTests.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamScheduleTests;

using ExamSchedule.Core;
using ExamSchedule.Core.Models;
using ExamSchedule.Core.Queries;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Location queries tests.
/// </summary>
public class LocationQueriesTests
{
    private static readonly DbContextOptions<ScheduleContext> Options = new DbContextOptionsBuilder<ScheduleContext>()
        .UseInMemoryDatabase(databaseName: "postgres")
        .Options;

    private static readonly ScheduleContext DbContext = new(Options);
    private static readonly LocationQueries Queries = new(DbContext);

    private static readonly InputLocation InputLocation = new()
    {
        Classroom = "somewhere",
    };

    /// <summary>
    /// Insert location test.
    /// </summary>
    [Test]
    [Order(1)]
    public void InsertLocationTest()
    {
        Assert.DoesNotThrowAsync(() => Queries.InsertLocation(InputLocation));
    }

    /// <summary>
    /// Get location test.
    /// </summary>
    [Test]
    [Order(2)]
    public void GetLocationTest()
    {
        var id = Queries.GetLocations().Result.Last(
            location =>
                location.Classroom == InputLocation.Classroom).LocationId;
        var location = Queries.GetLocations(id).Result.First();
        Assert.That(location.Classroom, Is.EqualTo(InputLocation.Classroom));
    }

    /// <summary>
    /// Update location test.
    /// </summary>
    [Test]
    [Order(3)]
    public void UpdateLocationTest()
    {
        var id = Queries.GetLocations().Result.Last(
            location =>
                location.Classroom == InputLocation.Classroom).LocationId;

        var newLocation = new InputLocation()
        {
            Classroom = "location",
        };
        Assert.DoesNotThrowAsync(() => Queries.UpdateLocation(id, newLocation));

        var location = Queries.GetLocations(id).Result.First();
        Assert.That(location.Classroom, Is.EqualTo(newLocation.Classroom));
        Assert.DoesNotThrowAsync(() => Queries.UpdateLocation(id, InputLocation));
    }

    /// <summary>
    /// Delete location test.
    /// </summary>
    [Test]
    [Order(4)]
    public void DeleteLocationTest()
    {
        var id = Queries.GetLocations().Result.Last(
            location =>
                location.Classroom == InputLocation.Classroom).LocationId;

        Assert.DoesNotThrowAsync(() => Queries.DeleteLocation(id));

        var location = Queries.GetLocations(id).Result.FirstOrDefault();
        Assert.That(location, Is.EqualTo(null));
    }
}