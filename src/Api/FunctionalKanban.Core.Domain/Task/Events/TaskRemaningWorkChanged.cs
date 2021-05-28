namespace FunctionalKanban.Core.Domain.Task.Events
{
    using System;
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;

    public sealed record TaskRemaningWorkChanged : Event
    {
        public uint RemaningWork { get; init; }

        public uint OldRemaningWork { get; init; }

        public Option<Guid> ProjectId { get; init; }
    }
}
