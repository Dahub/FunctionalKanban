namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using Unit = System.ValueTuple;

    public interface IEventDataBase
    {
        IEnumerable<Event> Events { get; }

        Exceptional<Unit> Add(Guid aggregateId, string aggregateName, uint aggregateVersion, string eventName, Event @event);
    }
}
