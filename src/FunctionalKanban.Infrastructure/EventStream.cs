namespace FunctionalKanban.Infrastructure
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;

    public class EventStream : IEventStream
    {
        private readonly IEventDataBase _database;

        public EventStream(IEventDataBase database) => _database = database;

        public Exceptional<Unit> Push(Event @event) =>
            _database.Add(
                Guid.NewGuid(),
                @event.EntityName,
                @event.EntityVersion,
                @event.EventName,
                @event);
    }
}
