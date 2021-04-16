﻿namespace FunctionalKanban.Domain.Task.ViewProjections
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    public record TaskViewProjection : ViewProjection
    {
        public TaskViewProjection() => Name = string.Empty;

        public string Name { get; init; }

        public uint RemaningWork { get; init; }

        public TaskStatus Status { get; init; }

        public Option<Guid> ProjectId { get; init; }

        public static bool CanHandle(Event @event) => 
            @event is TaskCreated or TaskStatusChanged or TaskDeleted;

        public override Option<ViewProjection> With(Event @event) =>
            @event switch
            {
                TaskCreated e       => this with { Id = e.EntityId, Name = e.Name, RemaningWork = e.RemaningWork, Status = e.Status, ProjectId = e.ProjectId },
                TaskStatusChanged e => this with { RemaningWork = e.RemaningWork, Status = e.NewStatus },
                TaskDeleted _       => None,
                _                   => this with { }
            };
    }
}
