namespace FunctionalKanban.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using static FunctionalKanban.Functional.F;

    public class EntityStateRepository : IEntityStateRepository
    {
        private readonly IEventDataBase _database;

        public EntityStateRepository(IEventDataBase database) => _database = database;

        public Exceptional<Option<State>> GetById(Guid id) =>
            GetEventsById(_database.Events, id)
                .Bind(WithEntityType)
                .Bind(WithStateInstance)
                .Bind(WithEvents)
                .Bind(WithFromExecution).Run();

        private static Try<Option<(State, IEnumerable<Event>)>> WithStateInstance(Option<(Type entityType, IEnumerable<Event> events)> tuple) =>
           Try(() =>
           {
               return tuple.Match(
                   None: () => None,
                   Some: (tuple) =>
                   {
                       var state = (State)Activator.CreateInstance(tuple.entityType);
                       return Some((state, tuple.events));
                   });
           });

        private static Try<Option<State>> WithFromExecution(Option<(State state, IEnumerable<Event> events)> tuple) =>
            Try(() => tuple.Match(
                    None: () => None,
                    Some: (tuple) => tuple.state.From(tuple.events)
                ));

        private static Try<Option<(State state, IEnumerable<Event> lines)>> WithEvents(Option<(State state, IEnumerable<Event> events)> tuple) =>
            Try(() =>
            {
                return tuple.Match(
                    None: () => None,
                    Some: (tuple) => Some((tuple.state, tuple.events)));
            });

        private static Try<Option<(Type, IEnumerable<Event>)>> WithEntityType(Option<IEnumerable<Event>> events) =>
            Try<Option<(Type, IEnumerable<Event>)>>(() =>
            {
                return events.Match(
                    None: () => None,
                    Some: (e) =>
                    {
                        var entityName = e.First().AggregateName;
                        var entityType = Assembly.GetAssembly(typeof(State)).GetType(entityName);
                        return entityType == null ? None : Some((entityType, e));
                    });
            });

        private static Try<Option<IEnumerable<Event>>> GetEventsById(IEnumerable<Event> allEvents, Guid id) =>
            Try(() =>
            {
                var events = allEvents.Where(e => e.AggregateId.Equals(id));
                return events.Any()
                    ? Some(events)
                    : None;
            });
    }
}
