namespace FunctionalKanban.Infrastructure.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.ViewProjections;
    using FunctionalKanban.Infrastructure.Abstraction;
    using LaYumba.Functional;
    using static FunctionalKanban.Infrastructure.InMemory.InMemoryDatabase;
    using static LaYumba.Functional.F;
    using Unit = System.ValueTuple;

    public class InMemoryDatabase : IViewProjectionDataBase, IEventStore
    {
        private readonly List<EventLine> _eventLines;

        private readonly ConcurrentDictionary<Guid, TaskViewProjection> _taskViewProjections;

        private readonly ConcurrentDictionary<Guid, ProjectViewProjection> _projectViewProjections;

        public InMemoryDatabase()
        {
            _eventLines = new List<EventLine>();
            _taskViewProjections = new ConcurrentDictionary<Guid, TaskViewProjection>();
            _projectViewProjections = new ConcurrentDictionary<Guid, ProjectViewProjection>();
        }

        public IEnumerable<Event> Events => _eventLines.Select(l => l.Data).ToList().AsReadOnly();

        public IEnumerable<TaskViewProjection> TaskViewProjections => _taskViewProjections.Values.ToList().AsReadOnly();

        public IEnumerable<ProjectViewProjection> ProjectViewProjections => _projectViewProjections.Values.ToList().AsReadOnly();

        public Exceptional<IEnumerable<T>> Projections<T>() where T : ViewProjection =>
            Projections(typeof(T)).Bind(projections => Convert<T>(projections));

        public Exceptional<IEnumerable<ViewProjection>> Projections(Type type)
        {
            if (type == typeof(TaskViewProjection))
            {
                return Exceptional((IEnumerable<ViewProjection>)_taskViewProjections.Values);
            }
            else if (type == typeof(ProjectViewProjection))
            {
                return Exceptional((IEnumerable<ViewProjection>)_projectViewProjections.Values);
            }

            return new Exception($"projection de type {type} non prise en charge");
        }

        public Exceptional<Unit> AddRange(IEnumerable<(Guid entityId, string entityName, uint entityVersion, string eventName, Event @event)> events) =>
            events.Aggregate(
                seed: Exceptional(Unit.Create()),
                func: (ex, next) => ex.Bind(_ => Add(next.entityId, next.entityName, next.entityVersion, next.eventName, next.@event)));

        public Exceptional<Unit> Add(
            Guid entityId,
            string entityName,
            uint entityVersion,
            string eventName,
            Event @event) =>
                @event.CheckUnicity(_eventLines).Bind(AddEventToLines);

        public Exceptional<Unit> Upsert<T>(T viewProjection) where T : ViewProjection =>
            viewProjection switch
            {
                TaskViewProjection p => Try(() => UpsertTaskViewProjection(p)).Run(),
                ProjectViewProjection p => Try(() => UpsertProjectViewProjection(p)).Run(),
                _ => new Exception($"Impossible d'insérer le type de projection {typeof(T)}")
            };

        public Exceptional<Unit> Delete<T>(T viewProjection) where T : ViewProjection =>
            Try(() =>
            {
                if (typeof(T) == typeof(TaskViewProjection))
                {
                    _taskViewProjections.TryRemove(viewProjection.Id, out _);
                    return Unit.Create();
                }
                else if (typeof(T) == typeof(ProjectViewProjection))
                {
                    _projectViewProjections.TryRemove(viewProjection.Id, out _);
                    return Unit.Create();
                }

                throw new Exception($"projection de type {typeof(T)} non prise en charge");
            }).Run();

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

        private Unit UpsertProjectViewProjection(ProjectViewProjection p)
        {
            if (_projectViewProjections.ContainsKey(p.Id))
            {
                _projectViewProjections[p.Id] = p;
            }
            else
            {
                _projectViewProjections.TryAdd(p.Id, p);
            }

            return Unit.Create();
        }

        private readonly Func<(Event, List<EventLine>), Exceptional<Unit>> AddEventToLines = (tuple) =>
            Try(() =>
            {
                tuple.Item2.Add(tuple.Item1.ToEventLine());
                return Unit.Create();
            }).Run();

        private static Exceptional<IEnumerable<T>> Convert<T>(IEnumerable<ViewProjection> projections) where T : ViewProjection =>
            Try(() => projections.Map(p => (T)p)).Run();
  
        internal record EventLine(
          Guid Id,
          Guid EntityId,
          string EntityName,
          uint Version,
          string EventName,
          DateTime TimeStamp,
          Event Data);
    }

    internal static class InMemoryDatabaseExt
    {
        public static EventLine ToEventLine(this Event evt) =>
            new(
                Id: Guid.NewGuid(),
                EntityId: evt.EntityId,
                EntityName: evt.EntityName,
                Version: evt.EntityVersion,
                EventName: evt.EventName,
                TimeStamp: evt.TimeStamp,
                Data: evt);

        public static Exceptional<(Event, List<EventLine>)> CheckUnicity(this Event @event, List<EventLine> lines) =>
            Try(() =>
                lines.Where(l => l.EntityId.Equals(@event.EntityId)
                     && l.EntityName.Equals(@event.EntityName)
                     && l.Version.Equals(@event.EntityVersion)).Any()
                ? throw new AggregateException("Un événement pour cette version d'entité est déjà présent")
                : (@event, lines)).Run();
    }
}
