namespace FunctionalKanban.Domain.Project
{
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project.Events;
    using LaYumba.Functional;

    public sealed record ProjectEntityState : State
    {
        public ProjectEntityState() => ProjectName = string.Empty;

        public string ProjectName { get; init; }

        public ProjectStatus ProjectStatus { get; init; }

        public bool IsDeleted { get; init; }

        public override Option<State> From(IEnumerable<Event> history) =>
            From<ProjectCreated>(history, () => new ProjectEntityState());

        protected override State With(Event @event) =>
            @event switch
            {
                ProjectCreated e    => this with { Version = e.EntityVersion, ProjectName = e.Name, ProjectStatus = e.Status, IsDeleted = e.IsDeleted },
                _                   => this with { }
            };
    }
}
