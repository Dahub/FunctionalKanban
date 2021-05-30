namespace FunctionalKanban.Infrastructure.SqlServer.EventDatabase
{
    using System.Diagnostics.CodeAnalysis;
    using FunctionalKanban.Infrastructure.SqlServer.EventDatabase.EfEntities;
    using Microsoft.EntityFrameworkCore;

    public class EventDbContext : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public EventDbContext([NotNull] DbContextOptions options) : base(options) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<EventEfEntity>().ToTable("Events");
            modelBuilder.Entity<EventEfEntity>().HasKey(r => r.Id);
            modelBuilder.Entity<EventEfEntity>()
                .Property(r => r.Id)
                .HasColumnName("Id")
                .HasColumnType("uniqueIdentifier")
                .IsRequired();
            modelBuilder.Entity<EventEfEntity>()
                .Property(r => r.EntityId)
                .HasColumnName("EntityId")
                .HasColumnType("uniqueIdentifier")
                .IsRequired();
            modelBuilder.Entity<EventEfEntity>()
               .Property(r => r.EntityName)
               .HasColumnName("EntityName")
               .HasColumnType("nvarchar(512)")
               .IsRequired();
            modelBuilder.Entity<EventEfEntity>()
                .Property(r => r.EventDatas)
                .HasColumnName("Datas")
                .HasColumnType("varbinary(max)");
            modelBuilder.Entity<EventEfEntity>()
                .Property(r => r.EventName)
                .HasColumnName("Name")
                .HasColumnType("nvarchar(512)")
                .IsRequired();
            modelBuilder.Entity<EventEfEntity>()
                .Property(r => r.TimeStamp)
                .HasColumnName("CreationDate")
                .HasColumnType("datetime")
                .IsRequired();
            modelBuilder.Entity<EventEfEntity>()
                .Property(r => r.Version)
                .HasColumnName("Version")
                .HasColumnType("integer")
                .IsRequired();
            modelBuilder.Entity<EventEfEntity>()
                .HasIndex(e => new
                {
                    e.EntityId,
                    e.EntityName,
                    e.Version
                })
                .IsUnique(true);
            modelBuilder.Entity<EventEfEntity>()
                .HasIndex(e => e.EntityId);
        }

        internal DbSet<EventEfEntity> Events { get; set; }
    }
}
