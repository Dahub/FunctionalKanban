namespace FunctionalKanban.Infrastructure.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;
    using static FunctionalKanban.Functional.F;

    public class InMemoryEventStream : IEventStream
    {
        private readonly IInMemoryDatabase _inMemoryDataBase;

        public InMemoryEventStream(IInMemoryDatabase inMemoryDatabase) => _inMemoryDataBase = inMemoryDatabase;

        public Exceptional<Unit> Push(Event @event) =>
            @event.CheckUnicity(_inMemoryDataBase.EventLines).Bind(e => e.AppendToLines(_inMemoryDataBase)).Run();
    }

    public record EventLine(
        Guid id,
        Guid aggregateId,
        string aggregateName,
        uint version,
        string eventName,
        DateTime timeStamp,
        Event data);

    internal static class InMemoryEventStreamExt
    {
        public static Try<Event> CheckUnicity(this Event @event, IEnumerable<EventLine> lines) =>
            Try(() => 
                lines.Where(l => l.aggregateId.Equals(@event.AggregateId)
                     && l.aggregateName.Equals(@event.AggregateName)
                     && l.version.Equals(@event.EntityVersion)).Any()
                ?throw new AggregateException("Un événement pour cette version d'aggregat est déjà présent")
                :@event);

        public static Try<Unit> AppendToLines(this Event @event, IInMemoryDatabase dataBase) =>
            Try(() =>
            {
                dataBase.Add(new EventLine(
                      id: Guid.NewGuid(),
                      aggregateId: @event.AggregateId,
                      aggregateName: @event.AggregateName,
                      version: @event.EntityVersion,
                      eventName: @event.EventName,
                      timeStamp: @event.TimeStamp,
                      data: @event));

                return Unit.Create();
            });
    }
}
