namespace FunctionalKanban.Infrastructure.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Infrastructure.Abstraction;
    using FunctionalKanban.Infrastructure.SqlServer.EfEntities;
    using LaYumba.Functional;
    using Microsoft.EntityFrameworkCore;

    internal class EventDbContext : DbContext, IEventDataBase
    {
        public EventDbContext([NotNull] DbContextOptions options) : base(options) { }

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
        }

        public DbSet<EventEfEntity>? Events { get; set; }

        public Exceptional<ValueTuple> Add(Guid entityId, string entityName, uint entityVersion, string eventName, Event @event) => throw new NotImplementedException();

        public IEnumerable<Event> EventsByEntityId(Guid entityId) => throw new NotImplementedException();

        public Task Commit() => base.SaveChangesAsync();

        public Task Rollback() => Task.CompletedTask;
    }
}
