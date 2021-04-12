namespace FunctionalKanban.Domain.Task.Events
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;

    public sealed record TaskDeleted : Event
    {
        public bool IsDeleted { get; init; }

        public Option<Guid> ProjectId { get; init; }
    }
}
