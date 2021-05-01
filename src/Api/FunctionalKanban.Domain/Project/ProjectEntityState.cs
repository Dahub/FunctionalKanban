namespace FunctionalKanban.Domain.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project.Events;

    public sealed record ProjectEntityState : State
    {
        public ProjectEntityState() => ProjectName = string.Empty;

        public Guid ProjectId { get; init; }

        public string ProjectName { get; init; }

        public ProjectStatus ProjectStatus { get; init; }

        public bool IsDeleted { get; init; }

        private List<Guid> _associatedTaskIds = Enumerable.Empty<Guid>().ToList();

        public IEnumerable<Guid> AssociatedTaskIds 
        {
            get => _associatedTaskIds.AsReadOnly();
            init => _associatedTaskIds = value.ToList();
        }

        protected override State With(Event @event) =>
            @event switch
            {
                ProjectCreated e        => this with { ProjectId = e.EntityId, Version = e.EntityVersion, ProjectName = e.Name, ProjectStatus = e.Status, IsDeleted = e.IsDeleted },
                ProjectNewTaskLinked e  => this with { Version = e.EntityVersion, AssociatedTaskIds = AssociatedTaskIds.Append(e.TaskId) },
                _                       => this with { }
            };
    }
}
