namespace FunctionalKanban.Infrastructure.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using static FunctionalKanban.Functional.F;
    using static FunctionalKanban.Infrastructure.InMemory.InMemoryDatabase;
    using Unit = System.ValueTuple;

    public class InMemoryDatabase : IViewProjectionDataBase, IEventDataBase
    {
        private readonly List<EventLine> _eventLines;

        private readonly ConcurrentDictionary<Guid, TaskViewProjection> _taskViewProjections;

        public InMemoryDatabase()
        {
            _eventLines = new List<EventLine>();
            _taskViewProjections = new ConcurrentDictionary<Guid, TaskViewProjection>();
        }

        public IEnumerable<Event> Events => _eventLines.Select(l => l.Data).ToList().AsReadOnly();

        public IEnumerable<TaskViewProjection> TaskViewProjections => _taskViewProjections.Values.ToList().AsReadOnly();

        public Exceptional<Unit> Add(
            Guid aggregateId, 
            string aggregateName,
            uint aggregateVersion, 
            string eventName, 
            Event @event) =>
                @event.CheckUnicity(_eventLines).Bind(AddEventToLines);

        public void Upsert(TaskViewProjection viewProjection)
        {
            if(_taskViewProjections.ContainsKey(viewProjection.Id))
            {
                _taskViewProjections[viewProjection.Id] = viewProjection;
            }
            else
            {
                _taskViewProjections.TryAdd(viewProjection.Id, viewProjection);
            }
        }

        private Func<(Event, List<EventLine>), Exceptional<Unit>> AddEventToLines = (tuple) =>
            Try(() =>
            {
                tuple.Item2.Add(tuple.Item1.ToEventLine());
                return Unit.Create();
            }).Run();

        internal record EventLine(
          Guid Id,
          Guid AggregateId,
          string AggregateName,
          uint Version,
          string EventName,
          DateTime TimeStamp,
          Event Data);
    }

    internal static class InMemoryDatabaseExt
    {
        public static EventLine ToEventLine(this Event evt) =>
            new EventLine(
                Id: Guid.NewGuid(),
                AggregateId: evt.AggregateId,
                AggregateName: evt.AggregateName,
                Version: evt.EntityVersion,
                EventName: evt.EventName,
                TimeStamp: evt.TimeStamp,
                Data: evt);

        public static Exceptional<(Event, List<EventLine>)> CheckUnicity(this Event @event, List<EventLine> lines) =>
            Try(() =>
                lines.Where(l => l.AggregateId.Equals(@event.AggregateId)
                     && l.AggregateName.Equals(@event.AggregateName)
                     && l.Version.Equals(@event.EntityVersion)).Any()
                ? throw new AggregateException("Un événement pour cette version d'aggregat est déjà présent")
                : (@event, lines)).Run();
    }
}
