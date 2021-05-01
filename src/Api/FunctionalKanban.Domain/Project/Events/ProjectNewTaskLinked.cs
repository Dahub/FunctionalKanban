namespace FunctionalKanban.Domain.Project.Events
{
    using System;
    using FunctionalKanban.Domain.Common;

    public sealed record ProjectNewTaskLinked : Event
    {
        public Guid TaskId { get; init; }
    }
}
