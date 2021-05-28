namespace FunctionalKanban.Core.Domain.Project.Events
{
    using System;
    using FunctionalKanban.Core.Domain.Common;

    public sealed record ProjectNewTaskLinked : Event
    {
        public Guid TaskId { get; init; }
    }
}
