namespace FunctionalKanban.Infrastructure.InMemory
{
    using System.Collections.Generic;

    public static class InMemoryDatabase
    {
        public static IEnumerable<EventLine> EventLines => _lines.AsReadOnly();

        internal readonly static List<EventLine> _lines = new List<EventLine>();

        public static void Reset() => _lines.Clear();
    }
}
