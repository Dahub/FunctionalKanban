namespace FunctionalKanban.Infrastructure
{
    using System;
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;

    public class EventStream : IEventStream
    {
        private readonly IEventStore _database;

        public EventStream(IEventStore database) => _database = database;

        public Exceptional<Unit> Push(params Event[] @event)
        {
            var tt = _database.AddRange(@event.Map(e => (
             Guid.NewGuid(),
             e.EntityName,
             e.EntityVersion,
             e.EventName,
             e)));

            return tt;
        }
         
    }
}
