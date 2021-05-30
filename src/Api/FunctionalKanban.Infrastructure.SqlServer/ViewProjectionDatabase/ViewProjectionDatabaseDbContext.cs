namespace FunctionalKanban.Infrastructure.SqlServer.ViewProjectionDatabase
{
    using System.Diagnostics.CodeAnalysis;
    using FunctionalKanban.Core.Domain.ViewProjections;
    using FunctionalKanban.Infrastructure.SqlServer.ViewProjectionDatabase.EfEntities;
    using Microsoft.EntityFrameworkCore;

    public class ViewProjectionDatabaseDbContext : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ViewProjectionDatabaseDbContext([NotNull] DbContextOptions options) : base(options) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<DeletedTask>().ToTable("DeletedTasks");
            modelBuilder.Entity<DeletedTask>().HasKey(r => r.Id);
            modelBuilder.Entity<DeletedTask>()
                .Property(r => r.Id)
                .HasColumnName("Id")
                .HasColumnType("uniqueIdentifier")
                .IsRequired();
            modelBuilder.Entity<DeletedTask>()
                .Property(r => r.DeletedAt)
                .HasColumnName("DeletedAt")
                .HasColumnType("datetime")
                .IsRequired();

            modelBuilder.Entity<Project>().ToTable("Projects");
            modelBuilder.Entity<Project>().HasKey(r => r.Id);
            modelBuilder.Entity<Project>()
                .Property(r => r.Id)
                .HasColumnName("Id")
                .HasColumnType("uniqueIdentifier")
                .IsRequired();
            modelBuilder.Entity<Project>()
                .Property(r => r.IsDeleted)
                .HasColumnName("IsDeleted")
                .HasColumnType("bit")
                .IsRequired();
            modelBuilder.Entity<Project>()
                .Property(r => r.Name)
                .HasColumnName("Name")
                .HasColumnType("nvarchar(512)")
                .IsRequired();
            modelBuilder.Entity<Project>()
                .Property(r => r.Status)
                .HasColumnName("Status")
                .HasColumnType("integer")
                .IsRequired();
            modelBuilder.Entity<Project>()
                .Property(r => r.TotalRemaningWork)
                .HasColumnName("TotalRemaningWork")
                .HasColumnType("integer")
                .IsRequired();

            modelBuilder.Entity<Task>().ToTable("Tasks");
            modelBuilder.Entity<Task>().HasKey(r => r.Id);
            modelBuilder.Entity<Task>()
                .Property(r => r.Id)
                .HasColumnName("Id")
                .HasColumnType("uniqueIdentifier")
                .IsRequired();
            modelBuilder.Entity<Task>()
                .Property(r => r.Name)
                .HasColumnName("Name")
                .HasColumnType("nvarchar(512)")
                .IsRequired();
            modelBuilder.Entity<Task>()
                .Property(r => r.ProjectId)
                .HasColumnName("ProjectId")
                .HasColumnType("uniqueIdentifier");
            modelBuilder.Entity<Task>()
                .Property(r => r.RemaningWork)
                .HasColumnName("RemaningWork")
                .HasColumnType("integer")
                .IsRequired();
            modelBuilder.Entity<Task>()
               .Property(r => r.Status)
               .HasColumnName("Status")
               .HasColumnType("integer")
               .IsRequired();
        }

        internal DbSet<DeletedTask> DeletedTasks { get; set; }

        internal DbSet<Project> Projects { get; set; }

        internal DbSet<Task> Tasks { get; set; }
    }
}
