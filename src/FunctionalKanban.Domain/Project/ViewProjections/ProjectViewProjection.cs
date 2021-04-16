﻿namespace FunctionalKanban.Domain.Project.ViewProjections
{
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Project.Events;
    using FunctionalKanban.Domain.Task.Events;

    public record ProjectViewProjection : ViewProjection
    {
        public ProjectViewProjection() => Name = string.Empty;

        public string Name { get; init; }

        public ProjectStatus Status { get; init; }

        public bool IsDeleted { get; init; }

        public uint TotalRemaningWork { get; init; }

        public static bool CanHandle(Event @event) =>
            @event is ProjectCreated or TaskCreated or TaskDeleted;

        public override ViewProjection With(Event @event) =>
            @event switch
            {
                ProjectCreated e    => this with { Id = e.EntityId, Name = e.Name, Status = e.Status, IsDeleted = e.IsDeleted, TotalRemaningWork = 0 },
                TaskCreated e       => this with { TotalRemaningWork = this.TotalRemaningWork + e.RemaningWork },
                TaskDeleted e       => this with { TotalRemaningWork = this.TotalRemaningWork - e.RemaningWork },
                _                   => this with { }
            };
    }
}
