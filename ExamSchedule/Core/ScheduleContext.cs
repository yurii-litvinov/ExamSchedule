// <copyright file="ScheduleContext.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace ExamSchedule.Core;

using ExamSchedule.Core.Models;
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
    /// Gets or sets Staff table.
    /// </summary>
    public virtual DbSet<Staff> Staffs { get; set; } = null!;

    /// <summary>
    /// Gets or sets Role table.
    /// </summary>
    public virtual DbSet<Role> Roles { get; set; } = null!;

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
    /// Gets or sets Location table.
    /// </summary>
    public virtual DbSet<Location> Locations { get; set; } = null!;

    /// <summary>
    /// Gets or sets Student table.
    /// </summary>
    public virtual DbSet<Student> Students { get; set; } = null!;

    /// <summary>
    /// Gets or sets Student_Group table.
    /// </summary>
    public virtual DbSet<StudentGroup> StudentsGroups { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exam>(
            entity =>
            {
                entity.HasOne(exam => exam.Location).WithMany(location => location.Exams)
                    .HasForeignKey(exam => exam.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("location_fk");

                entity.HasOne(exam => exam.Student).WithMany(student => student.Exams)
                    .HasForeignKey(exam => exam.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_fk");

                entity.HasOne(exam => exam.Type).WithMany(examType => examType.Exams)
                    .HasForeignKey(exam => exam.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("type_fk");
            });

        modelBuilder.Entity<ExamLecturer>(
            entity =>
            {
                entity.HasOne(examLecturer => examLecturer.Exam).WithMany()
                    .HasForeignKey(examLecturer => examLecturer.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("exam_fk");

                entity.HasOne(examLecturer => examLecturer.Lecturer).WithMany()
                    .HasForeignKey(examLecturer => examLecturer.LecturerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("lecturer_fk");
            });
    }
}