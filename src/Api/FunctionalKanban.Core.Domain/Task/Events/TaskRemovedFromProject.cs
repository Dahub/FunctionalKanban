namespace FunctionalKanban.Core.Domain.Task.Events
{
    using System;
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;

    public sealed record TaskRemovedFromProject : Event
    {
        public Option<Guid> OldProjectId { get; init; }

        public Option<Guid> ProjectId { get; init; }

        public uint RemaningWork { get; init; }
    }
}
