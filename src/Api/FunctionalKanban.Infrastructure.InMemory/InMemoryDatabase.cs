namespace FunctionalKanban.Infrastructure.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.ViewProjections;
    using FunctionalKanban.Infrastructure.Abstraction;
    using LaYumba.Functional;
    using static FunctionalKanban.Infrastructure.InMemory.InMemoryDatabase;
    using static LaYumba.Functional.F;
    using Unit = System.ValueTuple;

    public class InMemoryDatabase : IViewProjectionDataBase, IEventDataBase
    {
        private readonly List<EventLine> _eventLines;

        private readonly IDictionary<string, ConcurrentDictionary<Guid, ViewProjection>> _dbSets;

        public InMemoryDatabase()
        {
            _eventLines = new List<EventLine>();
            _dbSets = new Dictionary<string, ConcurrentDictionary<Guid, ViewProjection>>()
            {
                { typeof(TaskViewProjection).Name, new ConcurrentDictionary<Guid, ViewProjection>() },
                { typeof(ProjectViewProjection).Name, new ConcurrentDictionary<Guid, ViewProjection>() },
                { typeof(DeletedTaskViewProjection).Name, new ConcurrentDictionary<Guid, ViewProjection>() }
            };
        }

        public IEnumerable<Event> EventsByEntityId(Guid entityId) =>
             _eventLines.Where(e => e.EntityId.Equals(entityId)).Select(l => l.Data).ToList().AsReadOnly();

        public IEnumerable<T> GetProjections<T>() where T : ViewProjection =>
            _dbSets[typeof(T).Name] is ConcurrentDictionary<Guid, ViewProjection> dbSet
                ? dbSet.Values.Select(value => (T)value).ToList().AsReadOnly().AsEnumerable()
                : Enumerable.Empty<T>();

        public Exceptional<IEnumerable<T>> Projections<T>() where T : ViewProjection =>
            _dbSets[typeof(T).Name] is ConcurrentDictionary<Guid, ViewProjection> dbSet
                ? Exceptional(dbSet.Values.Select(value => (T)value).ToList().AsReadOnly().AsEnumerable())
                : new Exception($"projection de type {typeof(T).Name} non prise en charge");

        public Exceptional<IEnumerable<ViewProjection>> Projections(Type type, Func<ViewProjection, bool> predicate) =>
            _dbSets[type.Name] is ConcurrentDictionary<Guid, ViewProjection> dbSet
                ? Exceptional(dbSet.Values.Where(predicate).ToList().AsReadOnly().AsEnumerable())
                : new Exception($"projection de type {type} non prise en charge");

        public Exceptional<Unit> Add(Event @event) => @event.CheckUnicity(_eventLines).Bind(AddEventToLines);

        public Exceptional<Unit> Upsert<T>(T viewProjection) where T : ViewProjection =>
            Try(() =>
            {
                var dbSet = _dbSets[viewProjection.GetType().Name];
                if (dbSet.ContainsKey(viewProjection.Id))
                {
                    dbSet[viewProjection.Id] = viewProjection;
                }
                else
                {
                    dbSet.TryAdd(viewProjection.Id, viewProjection);
                }

                return Unit.Create();
            }).Run();

        public Exceptional<Unit> Delete<T>(T viewProjection) where T : ViewProjection =>
            Try(() => _dbSets[typeof(T).Name].TryRemove(viewProjection.Id, out _)
                ? Unit.Create()
                : throw new Exception("Erreur lors de la tentative de suppression de la projection")).Run();

        private readonly Func<(Event, List<EventLine>), Exceptional<Unit>> AddEventToLines = (tuple) =>
            Try(() =>
            {
                tuple.Item2.Add(tuple.Item1.ToEventLine());
                return Unit.Create();
            }).Run();

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
