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

        public Option<State> GetById(Guid id) =>
            GetEventLinesById(_inMemoryDataBase.EventLines, id)
                .Bind(WithEntityType)
                .Bind(WithFromMethod)
                .Bind(WithEvents)
                .Bind(WithMethodInvocation);

        private static Option<State> WithMethodInvocation((MethodInfo method, IEnumerable<Event> events) tuple) =>
            (Option<State>)tuple.method.Invoke(null, new object[] { tuple.events });

        private static Option<(MethodInfo method, IEnumerable<Event> lines)> WithEvents((MethodInfo method, IEnumerable<EventLine> lines) tuple) =>
            Some((tuple.method, tuple.lines.Select(el => el.data)));

        private static Option<(MethodInfo, IEnumerable<EventLine>)> WithFromMethod((Type entityType, IEnumerable<EventLine> lines) tuple)
        {
            var fromMethod = tuple.entityType.GetMethod("From");
            return fromMethod == null ? None : Some((fromMethod, tuple.lines));
        }

        private static Option<(Type, IEnumerable<EventLine>)> WithEntityType(IEnumerable<EventLine> lines)
        {
            var entityName = lines.First().aggregateName;
            var entityType = Assembly.GetAssembly(typeof(State)).GetType(entityName);
            return entityType == null ? None : Some((entityType, lines));
        }

        private static Option<IEnumerable<EventLine>> GetEventLinesById(IEnumerable<EventLine> allLines, Guid id)
        {
            var lines = allLines.Where(e => e.aggregateId.Equals(id));
            return lines.Any()
                ? Some(lines)
                : None;
        }
    }
}
