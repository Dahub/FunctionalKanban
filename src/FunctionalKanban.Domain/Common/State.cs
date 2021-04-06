namespace FunctionalKanban.Domain.Common
{
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    public abstract record State
    {
        public uint Version { get; init; }

        public abstract Option<State> From(IEnumerable<Event> history);

        public abstract Validation<State> ApplyEvent(Event @event);
    }
}
