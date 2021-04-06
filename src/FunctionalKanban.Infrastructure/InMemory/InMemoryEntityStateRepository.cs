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
                .Bind(WithFromMethod)
                .Bind(WithEvents)
                .Bind(WithMethodInvocation).Run();

        private static Try<Option<State>> WithMethodInvocation(Option<(MethodInfo method, IEnumerable<Event> events)> tuple) =>
            Try(() => tuple.Match(
                    None: () => None,
                    Some: (tuple) => (Option<State>)tuple.method.Invoke(null, new object[] { tuple.events })
                ));

        private static Try<Option<(MethodInfo method, IEnumerable<Event> lines)>> WithEvents(Option<(MethodInfo method, IEnumerable<EventLine> lines)> tuple) =>
            Try(() =>
            {
                return tuple.Match(
                    None: () => None,
                    Some: (tuple) => Some((tuple.method, tuple.lines.Select(el => el.data))));
            });

        private static Try<Option<(MethodInfo, IEnumerable<EventLine>)>> WithFromMethod(Option<(Type entityType, IEnumerable<EventLine> lines)> tuple) =>
            Try(() =>
            {
                return tuple.Match(
                    None: () => None,
                    Some: (tuple) =>
                    {
                        var fromMethod = tuple.entityType.GetMethod("From");
                        return fromMethod == null ? None : Some((fromMethod, tuple.lines));
                    });
            });

        private static Try<Option<(Type, IEnumerable<EventLine>)>> WithEntityType(Option<IEnumerable<EventLine>> lines) =>
            Try<Option<(Type, IEnumerable<EventLine>)>>(() =>
            {
                return lines.Match(
                    None: () => None,
                    Some: (l) =>
                    {
                        var entityName = l.First().aggregateName;
                        var entityType = Assembly.GetAssembly(typeof(State)).GetType(entityName);
                        return entityType == null ? None : Some((entityType, l));
                    });
            });

        private static Try<Option<IEnumerable<EventLine>>> GetEventLinesById(IEnumerable<EventLine> allLines, Guid id) =>
            Try(() =>
            {
                var lines = allLines.Where(e => e.aggregateId.Equals(id));
                return lines.Any()
                    ? Some(lines)
                    : None;
            });
    }
}
