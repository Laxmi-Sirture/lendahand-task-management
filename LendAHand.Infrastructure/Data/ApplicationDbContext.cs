using LendAHand.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<TaskItem> Tasks { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<FileUpload> FileUploads { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Role)
                    .HasConversion<string>();
            });

            // Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Designation).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.User)
                    .WithOne(u => u.Employee)
                    .HasForeignKey<Employee>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // TaskItem
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Priority).HasConversion<string>();
                entity.Property(e => e.Status).HasConversion<string>();
                entity.HasOne(e => e.AssignedEmployee)
                    .WithMany(emp => emp.Tasks)
                    .HasForeignKey(e => e.AssignedEmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // FileUpload
            modelBuilder.Entity<FileUpload>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
                entity.HasOne(e => e.Task)
                    .WithMany(t => t.FileUploads)
                    .HasForeignKey(e => e.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
