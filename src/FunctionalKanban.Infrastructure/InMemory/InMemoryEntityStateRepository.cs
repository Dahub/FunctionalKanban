namespace FunctionalKanban.Infrastructure.InMemory
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using static FunctionalKanban.Functional.F;

    public class InMemoryEntityStateRepository : IEntityStateRepository
    {
        public Option<State> GetById(Guid id) 
        {
            var eventsLines = InMemoryDatabase.EventLines.Where(e => e.aggregateId.Equals(id));
            if(!eventsLines.Any())
            {
                return None;
            }
            var entityName = eventsLines.First().aggregateName;

            var entityType = Assembly.GetAssembly(typeof(State)).GetType(entityName);
            if(entityType == null)
            {
                return None;
            }

            var fromMethod = entityType.GetMethod("From");
            if (fromMethod == null)
            {
                return None;
            }

            var events = eventsLines.Select(el => el.data);

            return (Option<State>)fromMethod.Invoke(null, new object[] { events });
        }
    }
}
