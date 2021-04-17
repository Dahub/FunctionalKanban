namespace FunctionalKanban.Domain.Task.Events
{
    using System;
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;

    public sealed record TaskLinkedToProject : Event
    {
        public Option<Guid> ProjectId { get; init; }
    }
}
