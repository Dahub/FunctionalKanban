namespace FunctionalKanban.Domain.Task.Events
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;

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
