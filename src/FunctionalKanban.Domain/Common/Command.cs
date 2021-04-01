namespace FunctionalKanban.Domain.Common
{
    using System;

    public record Command
    {
        public Guid AggregateId { get; init; }

        public DateTime TimeStamp { get; init; }
    }
}
