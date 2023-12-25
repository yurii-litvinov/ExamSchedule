// <copyright file="QueriesBase.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core.Queries;

using Npgsql;

/// <summary>
/// Queries base class.
/// </summary>
public class QueriesBase
{
    /// <summary>
    /// Instance of <see cref="NpgsqlConnection"/>.
    /// </summary>
    protected readonly NpgsqlConnection connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueriesBase"/> class.
    /// </summary>
    /// <param name="connection">Instance of <see cref="NpgsqlConnection"/>.</param>
    protected QueriesBase(NpgsqlConnection connection)
    {
        this.connection = connection;
    }
}