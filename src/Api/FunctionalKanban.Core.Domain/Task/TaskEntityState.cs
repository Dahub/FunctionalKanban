namespace FunctionalKanban.Core.Domain.Task
{
    using System;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.Task.Events;
    using LaYumba.Functional;

    public sealed record TaskEntityState : State
    {
        public TaskEntityState() => TaskName = string.Empty;

        public Guid TaskId { get; init; }

        public string TaskName { get; init; }

        public Option<Guid> ProjectId { get; init; }

        public TaskStatus TaskStatus { get; init; }

        public uint RemaningWork { get; init; }

        public bool IsDeleted { get; init; }
       
        protected override State With(Event @event) =>
            @event switch
            {
                TaskCreated e               => this with { Version = e.EntityVersion, TaskId = e.EntityId, RemaningWork = e.RemaningWork, IsDeleted = e.IsDeleted, TaskName = e.Name, TaskStatus = e.Status, ProjectId = e.ProjectId },
                TaskStatusChanged e         => this with { Version = e.EntityVersion, TaskStatus = e.NewStatus, RemaningWork = e.RemaningWork },
                TaskDeleted e               => this with { Version = e.EntityVersion, IsDeleted = e.IsDeleted, ProjectId = e.ProjectId, RemaningWork = e.RemaningWork },
                TaskRemaningWorkChanged e   => this with { Version = e.EntityVersion, RemaningWork = e.RemaningWork },
                TaskLinkedToProject e       => this with { Version = e.EntityVersion, ProjectId = e.ProjectId },
                TaskRemovedFromProject e    => this with { Version = e.EntityVersion, ProjectId = e.ProjectId },
                _                           => this with { }
            };
    }
}
