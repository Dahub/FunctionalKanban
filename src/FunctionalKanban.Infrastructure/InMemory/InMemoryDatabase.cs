namespace FunctionalKanban.Infrastructure.InMemory
{
    using System.Collections.Generic;

    public class InMemoryDatabase : IInMemoryDatabase
    {
        public InMemoryDatabase() => _lines = new List<EventLine>();

        public IEnumerable<EventLine> EventLines => _lines.AsReadOnly();

        private readonly List<EventLine> _lines;

        public void Add(EventLine evtLine) => _lines.Add(evtLine);
    }

    public interface IInMemoryDatabase
    {
        IEnumerable<EventLine> EventLines { get; }

        void Add(EventLine evtLine);
    }
}
