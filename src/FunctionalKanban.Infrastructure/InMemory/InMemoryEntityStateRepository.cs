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
        public Option<State> GetById(Guid id) =>
            GetEventLinesById(InMemoryDatabase.EventLines, id)
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
            if (fromMethod == null)
            {
                return None;
            }

            return Some((fromMethod, tuple.lines));
        }

        private static Option<(Type, IEnumerable<EventLine>)> WithEntityType(IEnumerable<EventLine> lines)
        {
            var entityName = lines.First().aggregateName;
            var entityType = Assembly.GetAssembly(typeof(State)).GetType(entityName);
            if (entityType == null)
            {
                return None;
            }
            return Some((entityType, lines));
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
