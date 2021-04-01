namespace FunctionalKanban.Domain.Common
{
    using System;

    public abstract record Event
    {
        public Guid AggregateId { get; init; }

        public string AggregateName { get; set; }

        public DateTime TimeStamp { get; init; }

        public uint EntityVersion { get; init; }
    }
}
