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
            CheckUnicity((_context, @event)).Bind(AddEventToContext);

        private static Exceptional<Unit> AddEventToContext((EventDbContext context, Event @event) tuple)
        {
            tuple.context.Add(tuple.@event.ToEfEntity());
            tuple.context.SaveChanges();
            return Unit.Create();
        }

        private static Exceptional<(EventDbContext, Event)> CheckUnicity((EventDbContext context, Event @event) tuple) =>
            Try(() =>
                tuple.context.Events.Any(
                    l => l.EntityId == tuple.@event.EntityId
                    && l.EntityName == tuple.@event.EntityName
                    && l.Version == tuple.@event.EntityVersion)
                ? throw new AggregateException("Un événement pour cette version d'entité est déjà présent")
                : tuple).Run();
    }
}
