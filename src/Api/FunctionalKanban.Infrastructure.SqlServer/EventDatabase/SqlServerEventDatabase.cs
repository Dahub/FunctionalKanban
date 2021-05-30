namespace FunctionalKanban.Infrastructure.SqlServer.EventDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Infrastructure.Abstraction;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;
    using Unit = System.ValueTuple;

    public class SqlServerEventDatabase : IEventDataBase
    {
        private readonly EventDbContext _context;

        public SqlServerEventDatabase(EventDbContext dbContext) => _context = dbContext;

        public IEnumerable<Event> EventsByEntityId(Guid entityId) =>
            _context.Events.Where(e => e.EntityId.Equals(entityId)).ToEvent();

        public Exceptional<Unit> Add(Event @event) =>
            CheckUnicity(@event, _context).Bind(_ => AddEventToContext(_context, @event));

        private static Exceptional<Unit> AddEventToContext(EventDbContext context, Event @event)
        {
            context.Add(@event.ToEventEfEntity());
            context.SaveChanges();
            return Unit.Create();
        }

        private static Exceptional<Unit> CheckUnicity(Event @event, EventDbContext context) =>
            Try(() =>
                context.Events.Any(
                    l => l.EntityId == @event.EntityId
                    && l.EntityName == @event.EntityName
                    && l.Version == @event.EntityVersion)
                ? throw new AggregateException("Un événement pour cette version d'entité est déjà présent")
                : Unit.Create()).Run();
    }
}
