namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;
    using Unit = System.ValueTuple;

    public interface IEventStore
    {
        IEnumerable<Event> Events { get; }

        Exceptional<Unit> Add(Guid entityId, string entityName, uint entityVersion, string eventName, Event @event);

        Exceptional<Unit> AddRange(IEnumerable<(Guid entityId, string entityName, uint entityVersion, string eventName, Event @event)> events);
    }
}
