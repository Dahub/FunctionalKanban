namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;
    using Unit = System.ValueTuple;

    public interface IEventDataBase
    {
        IEnumerable<Event> EventsByEntityId(Guid entityId);

        Exceptional<Unit> Add(Guid entityId, string entityName, uint entityVersion, string eventName, Event @event);

        Task Commit();

        Task Rollback();
    }
}
