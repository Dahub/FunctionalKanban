namespace FunctionalKanban.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;

    public class InMemoryEventStream : IEventStream
    {
        public IEnumerable<EventLine> EventLines => _lines.AsReadOnly();

        private readonly static List<EventLine> _lines = new List<EventLine>();

        public Exceptional<Unit> Push(Event @event) =>
            @event.CheckUnicity(_lines).Bind(e => e.AppendToLines(_lines));
    }

    public record EventLine(
        Guid id,
        Guid aggregateId,
        string aggregateName,
        uint version,
        DateTime timeStamp,
        Event data);

    internal static class InMemoryEventStreamExt
    {
        public static Exceptional<Event> CheckUnicity(this Event @event, IList<EventLine> lines)
            => lines.Where(l => l.aggregateId.Equals(@event.AggregateId)
                 && l.aggregateName.Equals(@event.AggregateName)
                 && l.version.Equals(@event.EntityVersion)).Any()
            ?new AggregateException("Un événement pour cette version d'aggregat est déjà présent")
            :@event;

        public static Exceptional<Unit> AppendToLines(this Event @event, IList<EventLine> lines)
        {
            lines.Add(new EventLine(
                  id:               Guid.NewGuid(),
                  aggregateId:      @event.AggregateId,
                  aggregateName:    @event.AggregateName,
                  version:          @event.EntityVersion,
                  timeStamp:        @event.TimeStamp,
                  data:             @event));

            return Unit.Create();
        }
    }
}
