namespace FunctionalKanban.Core.Domain.ViewProjections
{
    using System;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.Task;
    using FunctionalKanban.Core.Domain.Task.Events;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    public record TaskViewProjection : ViewProjection
    {
        public TaskViewProjection() => Name = string.Empty;

        public string Name { get; init; }

        public int RemaningWork { get; init; }

        public TaskStatus Status { get; init; }

        public Guid? EfProjectId
        {
            get => ProjectId.Match(
                Some: (id) => id,
                None: () => (Guid?)null);
            set => ProjectId = value == null ? None : Some(value.Value);
        }

        public Option<Guid> ProjectId { get; set; }

        public override Option<ViewProjection> With(Event @event) =>
            @event switch
            {
                TaskCreated e               => this with { Id = e.EntityId, Name = e.Name, RemaningWork = (int)e.RemaningWork, Status = e.Status, ProjectId = e.ProjectId },
                TaskStatusChanged e         => this with { RemaningWork = (int)e.RemaningWork, Status = e.NewStatus },
                TaskDeleted _               => None,
                TaskRemaningWorkChanged e   => this with { RemaningWork = (int)e.RemaningWork },
                TaskLinkedToProject e       => this with { ProjectId = e.ProjectId },
                TaskRemovedFromProject e    => this with { ProjectId = e.ProjectId },
                _                           => this with { }
            };
    }
}
