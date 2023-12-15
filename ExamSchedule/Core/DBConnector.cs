// <copyright file="DBConnector.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core;

using Dapper;
using Npgsql;

/// <summary>
/// Class for initializing database connection.
/// </summary>
public class DbConnector
{
    /// <summary>
    /// Instance of <see cref="NpgsqlConnection"/>.
    /// </summary>
    protected readonly NpgsqlConnection connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConnector"/> class.
    /// </summary>
    protected DbConnector()
    {
        var host = Environment.GetEnvironmentVariable("HOST");
        var username = Environment.GetEnvironmentVariable("USERNAME");
        var password = Environment.GetEnvironmentVariable("PASSWORD");
        var database = Environment.GetEnvironmentVariable("DATABASE");
        var connectionString = $"Host={host};" +
                               $"Username={username};" +
                               $"Password={password};" +
                               $"Database={database}";
        this.connection = new NpgsqlConnection(connectionString);
        this.connection.Open();
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
}