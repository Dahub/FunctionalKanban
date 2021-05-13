namespace FunctionalKanban.Domain.Task.Events
{
    using System;
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;

    public record TaskRemovedFromProject : Event
    {
        public Option<Guid> OldProjectId { get; init; }

        public Option<Guid> ProjectId { get; init; }

        public uint RemaningWork { get; init; }
    }
}
