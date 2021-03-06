namespace FunctionalKanban.Core.Domain.Task.Events
{
    using System;
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;

    public sealed record TaskCreated : Event
    {
        public TaskCreated() => Name = string.Empty;

        public string Name { get; init; }

        public TaskStatus Status { get; init; }

        public uint RemaningWork { get; init; }

        public bool IsDeleted { get; init; }

        public Option<Guid> ProjectId { get; init; }
    }
}
