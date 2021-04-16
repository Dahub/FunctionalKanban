﻿namespace FunctionalKanban.Infrastructure
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
            AllEventsOfEntity(_database.Events, id).
            Bind(WithEntityType).
            Bind(WithStateInstance).
            Bind(Hydrate).Run();

        private static Try<Option<IEnumerable<Event>>> AllEventsOfEntity(IEnumerable<Event> allEvents, Guid id) =>
            Try(() =>
            {
                var events = allEvents.Where(e => e.EntityId.Equals(id));
                return events.Any()
                    ? Some(events)
                    : None;
            });

        private static Try<Option<(State, IEnumerable<Event>)>> WithStateInstance(Option<(Type entityType, IEnumerable<Event> events)> tuple) =>
           Try(() =>  
                tuple.Match(
                    None: () => None,
                    Some: (tuple) =>
                    {
                        var state = Activator.CreateInstance(tuple.entityType);
                        return state == null?None:Some(((State)state, tuple.events));
                    }));

        private static Try<Option<State>> Hydrate(Option<(State state, IEnumerable<Event> events)> tuple) =>
            Try(() => 
                tuple.Match(
                    None: () => None,
                    Some: (tuple) => tuple.state.From(tuple.events)));

        private static Try<Option<(Type, IEnumerable<Event>)>> WithEntityType(Option<IEnumerable<Event>> events) =>
            Try<Option<(Type, IEnumerable<Event>)>>(() =>
                events.Match(
                    None: () => None,
                    Some: (e) =>
                    {
                        var entityName = e.First().EntityName;
                        var entityType = Assembly.GetAssembly(typeof(State))?.GetType(entityName);
                        return entityType == null ? None : Some((entityType, e));
                    }));      
    }
}
