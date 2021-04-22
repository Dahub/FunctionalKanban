namespace FunctionalKanban.Domain.ViewProjections
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Events;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    public record TaskViewProjection : ViewProjection
    {
        public TaskViewProjection() => Name = string.Empty;

        public string Name { get; init; }

        public uint RemaningWork { get; init; }

        public TaskStatus Status { get; init; }

        public Option<Guid> ProjectId { get; init; }

        public static Option<Guid> HandleWithId(Event @event) =>
           @event switch
           {
                TaskCreated 
                    or TaskDeleted 
                    or TaskRemaningWorkChanged 
                    or TaskLinkedToProject => @event.EntityId,
                _ => None
           };

        public override Option<ViewProjection> With(Event @event) =>
            @event switch
            {
                TaskCreated e               => this with { Id = e.EntityId, Name = e.Name, RemaningWork = e.RemaningWork, Status = e.Status, ProjectId = e.ProjectId },
                TaskStatusChanged e         => this with { RemaningWork = e.RemaningWork, Status = e.NewStatus },
                TaskDeleted _               => None,
                TaskRemaningWorkChanged e   => this with { RemaningWork = e.RemaningWork },
                TaskLinkedToProject e       => this with { ProjectId = e.ProjectId },
                _                           => this with { }
            };
    }
}
