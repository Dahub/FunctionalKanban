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

        public DbSet<EventEfEntity>? Events { get; set; }

        public Exceptional<ValueTuple> Add(Guid entityId, string entityName, uint entityVersion, string eventName, Event @event) => throw new NotImplementedException();
        
        public IEnumerable<Event> EventsByEntityId(Guid entityId) => throw new NotImplementedException();

        public Task Commit() => base.SaveChangesAsync();

        public Task Rollback() => Task.CompletedTask;
    }
}
