namespace FunctionalKanban.Domain.Common
{
    using System;

    public abstract record Event
    {
        public Guid EntityId { get; init; }

        public DateTime TimeStamp { get; init; }

        public int EntityVersion { get; init; }
    }
}
