namespace FunctionalKanban.Infrastructure.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using static FunctionalKanban.Functional.F;

    public class InMemoryEntityStateRepository : IEntityStateRepository
    {
        private readonly IInMemoryDatabase _inMemoryDataBase;

        public InMemoryEntityStateRepository(IInMemoryDatabase inMemoryDatabase) => _inMemoryDataBase = inMemoryDatabase;

        public Exceptional<Option<State>> GetById(Guid id) =>
            GetEventLinesById(_inMemoryDataBase.EventLines, id)
                .Bind(WithEntityType)
                .Bind(WithStateInstance)
                // .Bind(WithFromMethod)
                .Bind(WithEvents)
                .Bind(WithFromExecution).Run();

        private static Try<Option<(State, IEnumerable<EventLine>)>> WithStateInstance(Option<(Type entityType, IEnumerable<EventLine> lines)> tuple) =>
           Try(() =>
           {
               return tuple.Match(
                   None: () => None,
                   Some: (tuple) =>
                   {
                       var state = (State)Activator.CreateInstance(tuple.entityType);
                       return Some((state, tuple.lines));
                   });
           });

        private static Try<Option<State>> WithFromExecution(Option<(State state, IEnumerable<Event> events)> tuple) =>
            Try(() => tuple.Match(
                    None: () => None,
                    Some: (tuple) => (Option<State>)tuple.state.From(tuple.events)
                ));

        private static Try<Option<(State state, IEnumerable<Event> lines)>> WithEvents(Option<(State state, IEnumerable<EventLine> lines)> tuple) =>
            Try(() =>
            {
                return tuple.Match(
                    None: () => None,
                    Some: (tuple) => Some((tuple.state, tuple.lines.Select(el => el.Data))));
            });

        private static Try<Option<(Type, IEnumerable<EventLine>)>> WithEntityType(Option<IEnumerable<EventLine>> lines) =>
            Try<Option<(Type, IEnumerable<EventLine>)>>(() =>
            {
                return lines.Match(
                    None: () => None,
                    Some: (l) =>
                    {
                        var entityName = l.First().AggregateName;
                        var entityType = Assembly.GetAssembly(typeof(State)).GetType(entityName);
                        return entityType == null ? None : Some((entityType, l));
                    });
            });

        private static Try<Option<IEnumerable<EventLine>>> GetEventLinesById(IEnumerable<EventLine> allLines, Guid id) =>
            Try(() =>
            {
                var lines = allLines.Where(e => e.AggregateId.Equals(id));
                return lines.Any()
                    ? Some(lines)
                    : None;
            });
    }
}
