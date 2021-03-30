namespace FunctionalKanban.Domain.Common
{
    using System;

    public record Command
    {
        public DateTime TimeStamp { get; init; }
    }
}
