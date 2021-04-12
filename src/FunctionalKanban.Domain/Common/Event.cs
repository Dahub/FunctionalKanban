namespace FunctionalKanban.Domain.Common
{
    using System;

    public abstract record Event
    {
        public Event() => AggregateName = string.Empty;

        public Guid AggregateId { get; init; }

        public string AggregateName { get; set; }

        public DateTime TimeStamp { get; init; }

        public uint EntityVersion { get; init; }

        public string EventName => GetType().FullName??string.Empty;
    }
}
