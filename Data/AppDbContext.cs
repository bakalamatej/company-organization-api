using firmyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Division> Divisions { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ========================
        // Company - Employees (1:N)
        // ========================
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Company)
            .WithMany(c => c.Employees)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // ========================
        // Company - Divisions (1:N)
        // ========================
        modelBuilder.Entity<Division>()
            .HasOne(d => d.Company)
            .WithMany(c => c.Divisions)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // ========================
        // Division - Projects (1:N)
        // ========================
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Division)
            .WithMany(d => d.Projects)
            .HasForeignKey(p => p.DivisionId)
            .OnDelete(DeleteBehavior.Cascade);

        // ========================
        // Project - Departments (1:N)
        // ========================
        modelBuilder.Entity<Department>()
            .HasOne(d => d.Project)
            .WithMany(p => p.Departments)
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // ========================
        // Employee - Department (optional)
        // ========================
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // ========================
        // Leaders (optional) - no cascade delete
        // ========================
        modelBuilder.Entity<Company>()
            .HasOne(c => c.Leader)
            .WithMany()
            .HasForeignKey(c => c.LeaderId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Division>()
            .HasOne(d => d.Leader)
            .WithMany()
            .HasForeignKey(d => d.LeaderId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Project>()
            .HasOne(p => p.Leader)
            .WithMany()
            .HasForeignKey(p => p.LeaderId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Department>()
            .HasOne(d => d.Leader)
            .WithMany()
            .HasForeignKey(d => d.LeaderId)
            .OnDelete(DeleteBehavior.NoAction);

        base.OnModelCreating(modelBuilder);
    }
}
