namespace FunctionalKanban.Infrastructure.SqlServer.ViewProjectionDatabase
{
    using System.Diagnostics.CodeAnalysis;
    using FunctionalKanban.Core.Domain.ViewProjections;
    using Microsoft.EntityFrameworkCore;

    public class ViewProjectionDatabaseDbContext : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ViewProjectionDatabaseDbContext([NotNull] DbContextOptions options) : base(options) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<DeletedTaskViewProjection>().ToTable("DeletedTasks");
            modelBuilder.Entity<DeletedTaskViewProjection>().HasKey(r => r.Id);
            modelBuilder.Entity<DeletedTaskViewProjection>()
                .Property(r => r.Id)
                .HasColumnName("Id")
                .HasColumnType("uniqueIdentifier")
                .IsRequired();
            modelBuilder.Entity<DeletedTaskViewProjection>()
                .Property(r => r.DeletedAt)
                .HasColumnName("DeletedAt")
                .HasColumnType("datetime")
                .IsRequired();

            modelBuilder.Entity<ProjectViewProjection>().ToTable("Projects");
            modelBuilder.Entity<ProjectViewProjection>().HasKey(r => r.Id);
            modelBuilder.Entity<ProjectViewProjection>()
                .Property(r => r.Id)
                .HasColumnName("Id")
                .HasColumnType("uniqueIdentifier")
                .IsRequired();
            modelBuilder.Entity<ProjectViewProjection>()
                .Property(r => r.IsDeleted)
                .HasColumnName("IsDeleted")
                .HasColumnType("bit")
                .IsRequired();
            modelBuilder.Entity<ProjectViewProjection>()
                .Property(r => r.Name)
                .HasColumnName("Name")
                .HasColumnType("nvarchar(512)")
                .IsRequired();
            modelBuilder.Entity<ProjectViewProjection>()
                .Property(r => r.Status)
                .HasColumnName("Status")
                .HasColumnType("integer")
                .IsRequired();
            modelBuilder.Entity<ProjectViewProjection>()
                .Property(r => r.TotalRemaningWork)
                .HasColumnName("TotalRemaningWork")
                .HasColumnType("integer")
                .IsRequired();

            modelBuilder.Entity<TaskViewProjection>().ToTable("Tasks");
            modelBuilder.Entity<TaskViewProjection>().HasKey(r => r.Id);
            modelBuilder.Entity<TaskViewProjection>()
                .Property(r => r.Id)
                .HasColumnName("Id")
                .HasColumnType("uniqueIdentifier")
                .IsRequired();
            modelBuilder.Entity<TaskViewProjection>()
                .Property(r => r.Name)
                .HasColumnName("Name")
                .HasColumnType("nvarchar(512)")
                .IsRequired();
            modelBuilder.Entity<TaskViewProjection>()
                .Property(r => r.ProjectId)
                .HasColumnName("ProjectId")
                .HasColumnType("uniqueIdentifier");
            modelBuilder.Entity<TaskViewProjection>()
                .Property(r => r.RemaningWork)
                .HasColumnName("RemaningWork")
                .HasColumnType("integer")
                .IsRequired();
            modelBuilder.Entity<TaskViewProjection>()
               .Property(r => r.Status)
               .HasColumnName("Status")
               .HasColumnType("integer")
               .IsRequired();
        }

        public DbSet<DeletedTaskViewProjection> DeletedTasks { get; set; }

        public DbSet<ProjectViewProjection> Projects { get; set; }

        public DbSet<TaskViewProjection> Tasks { get; set; }
    }
}
