namespace FunctionalKanban.Core.Domain.ViewProjections
{
    using System;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.Project;
    using FunctionalKanban.Core.Domain.Project.Events;
    using FunctionalKanban.Core.Domain.Task.Events;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    public record ProjectViewProjection : ViewProjection
    {
        public ProjectViewProjection() => Name = string.Empty;

        public string Name { get; init; }

        public ProjectStatus Status { get; init; }

        public bool IsDeleted { get; init; }

        public uint TotalRemaningWork { get; init; }

        public static Option<Guid> HandleWithId(Event @event) =>
            @event switch
            {
                TaskCreated e               => e.ProjectId,
                TaskDeleted e               => e.ProjectId,
                TaskRemaningWorkChanged e   => e.ProjectId,
                TaskLinkedToProject e       => e.ProjectId,
                TaskRemovedFromProject e    => e.ProjectId,
                ProjectCreated e            => e.EntityId,
                ProjectDeleted e            => e.EntityId,
                _                           => None
            };

        public override Option<ViewProjection> With(Event @event) =>
            @event switch
            {
                ProjectCreated e            => this with { Id = e.EntityId, Name = e.Name, Status = e.Status, IsDeleted = e.IsDeleted, TotalRemaningWork = 0 },
                TaskCreated e               => this with { TotalRemaningWork = this.TotalRemaningWork + e.RemaningWork },
                TaskDeleted e               => this with { TotalRemaningWork = this.TotalRemaningWork - e.OldRemaningWork },
                TaskRemaningWorkChanged e   => this with { TotalRemaningWork = this.TotalRemaningWork + e.RemaningWork - e.OldRemaningWork },
                TaskLinkedToProject e       => this with { TotalRemaningWork = this.TotalRemaningWork + e.RemaningWork },
                TaskRemovedFromProject e    => this with { TotalRemaningWork = this.TotalRemaningWork - e.RemaningWork },
                ProjectDeleted _            => None,
                _                           => this with { }
            };
    }
}
