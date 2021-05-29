namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;
    using Unit = System.ValueTuple;

    public interface IEventDataBase
    {
        IEnumerable<Event> EventsByEntityId(Guid entityId);

        Exceptional<Unit> Add(Event @event);
    }
}
