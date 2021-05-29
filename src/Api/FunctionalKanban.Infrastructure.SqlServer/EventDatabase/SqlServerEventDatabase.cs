namespace FunctionalKanban.Infrastructure.SqlServer.EventDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Infrastructure.Abstraction;
    using LaYumba.Functional;
    using Unit = System.ValueTuple;

    public class SqlServerEventDatabase : IEventDataBase
    {
        private readonly EventDbContext _context;

        public SqlServerEventDatabase(EventDbContext dbContext) => _context = dbContext;

        public IEnumerable<Event> EventsByEntityId(Guid entityId) =>
            _context.Events.Where(e => e.EntityId.Equals(entityId)).ToEvent();

        public Exceptional<Unit> Add(Event @event) => AddEventToContext(_context, @event);

        private static Exceptional<Unit> AddEventToContext(EventDbContext context, Event @event)
        {
            context.Add(@event.ToEventEfEntity());
            return Unit.Create();
        }
    }
}
