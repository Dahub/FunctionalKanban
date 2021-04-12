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

        public Exceptional<IEnumerable<T>> Projections<T>() where T : ViewProjection =>
            Projections(typeof(T)).Bind(projections => Convert<T>(projections));

        private Exceptional<IEnumerable<T>> Convert<T>(IEnumerable<ViewProjection> projections) where T : ViewProjection =>
            Try(() => projections.Map(p => (T)p)).Run();

        public Exceptional<IEnumerable<ViewProjection>> Projections(Type type)
        {
            if (type == typeof(TaskViewProjection))
            {
                return Exceptional((IEnumerable<ViewProjection>)_taskViewProjections.Values);
            }

            return new Exception($"projection de type {type} non prise en charge");
        }

        public Exceptional<Unit> Add(
            Guid aggregateId,
            string aggregateName,
            uint aggregateVersion,
            string eventName,
            Event @event) =>
                @event.CheckUnicity(_eventLines).Bind(AddEventToLines);

        public Exceptional<Unit> Upsert<T>(T viewProjection) where T : ViewProjection =>
            viewProjection switch
            {
                TaskViewProjection p => Try(() => UpsertTaskViewProjection(p)).Run(),
                _ => new Exception($"Impossible d'insérer le type de projection {typeof(T)}")
            };

        private Unit UpsertTaskViewProjection(TaskViewProjection p)
        {
            if (_taskViewProjections.ContainsKey(p.Id))
            {
                _taskViewProjections[p.Id] = p;
            }
            else
            {
                _taskViewProjections.TryAdd(p.Id, p);
            }

            return Unit.Create();
        }

        private readonly Func<(Event, List<EventLine>), Exceptional<Unit>> AddEventToLines = (tuple) =>
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
