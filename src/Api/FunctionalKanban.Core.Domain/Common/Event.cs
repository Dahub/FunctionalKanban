namespace FunctionalKanban.Core.Domain.Common
{
    using System;

    public abstract record Event
    {
        public Event() => EntityName = string.Empty;

        public Guid EntityId { get; init; }

        public string EntityName { get; set; }

        public DateTime TimeStamp { get; init; }

        public uint EntityVersion { get; init; }

        public string EventName => GetType().FullName??string.Empty;
    }
}
