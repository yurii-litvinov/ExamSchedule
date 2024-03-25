// <copyright file="ScheduleContext.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ExamSchedule.Core;

using Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Context for db.
/// </summary>
public class ScheduleContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleContext"/> class.
    /// </summary>
    /// <param name="options">Options for db context.</param>
    public ScheduleContext(DbContextOptions<ScheduleContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets Employee table.
    /// </summary>
    public virtual DbSet<Employee> Employees { get; set; } = null!;

    /// <summary>
    /// Gets or sets Exam table.
    /// </summary>
    public virtual DbSet<Exam> Exams { get; set; } = null!;

    /// <summary>
    /// Gets or sets ExamLecturer table.
    /// </summary>
    public virtual DbSet<ExamLecturer> ExamLecturers { get; set; } = null!;

    /// <summary>
    /// Gets or sets ExamType table.
    /// </summary>
    public virtual DbSet<ExamType> ExamTypes { get; set; } = null!;

    /// <summary>
    /// Gets or sets Lecturer table.
    /// </summary>
    public virtual DbSet<Lecturer> Lecturers { get; set; } = null!;

    /// <summary>
    /// Gets or sets Location table.
    /// </summary>
    public virtual DbSet<Location> Locations { get; set; } = null!;

    /// <summary>
    /// Gets or sets Student table.
    /// </summary>
    public virtual DbSet<Student> Students { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exam>(
            entity =>
            {
                entity.HasOne(d => d.Location).WithMany(p => p.Exams)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("location_fk");

                entity.HasOne(d => d.Student).WithMany(p => p.Exams)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_fk");

                entity.HasOne(d => d.Type).WithMany(p => p.Exams)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("type_fk");
            });

        modelBuilder.Entity<ExamLecturer>(
            entity =>
            {
                entity.HasOne(d => d.Exam).WithMany()
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("exam_fk");

                entity.HasOne(d => d.Lecturer).WithMany()
                    .HasForeignKey(d => d.LecturerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("lecturer_fk");
            });
    }
}