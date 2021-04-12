namespace FunctionalKanban.Domain.Task.ViewProjections
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Functional;

    public record TaskViewProjection : ViewProjection
    {
        public TaskViewProjection() => Name = string.Empty;

        public string Name { get; init; }

        public uint RemaningWork { get; init; }

        public TaskStatus Status { get; init; }

        public bool IsDeleted { get; init; }

        public Option<Guid> ProjectId { get; init; }

        public static bool CanHandle(Event @event) => 
            @event is TaskCreated or TaskStatusChanged or TaskDeleted;
    }

    public static class TaskViewProjectionExt
    {
        public static TaskViewProjection With(this TaskViewProjection view, Event @event) =>
            @event switch
            {
                TaskCreated e       => view with { Id = e.AggregateId, Name = e.Name, RemaningWork = e.RemaningWork, Status = e.Status, IsDeleted = e.IsDeleted, ProjectId = e.ProjectId },
                TaskStatusChanged e => view with { RemaningWork = e.RemaningWork, Status = e.NewStatus },
                TaskDeleted e       => view with { IsDeleted = e.IsDeleted, ProjectId = e.ProjectId },
                _                   => view with { }
            };
    }
}
