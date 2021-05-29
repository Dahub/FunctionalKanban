namespace FunctionalKanban.Infrastructure.Implementation
{
    using System;
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;

    public class EventStream : IEventStream
    {
        private readonly IEventDataBase _database;

        public EventStream(IEventDataBase database) => _database = database;

        public Exceptional<Unit> Push(Event @event) => _database.Add(@event);
    }
}
