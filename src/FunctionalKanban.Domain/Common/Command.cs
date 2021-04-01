namespace FunctionalKanban.Domain.Common
{
    using System;

    public record Command
    {
        public Guid EntityId { get; init; }

        public DateTime TimeStamp { get; init; }
    }
}
