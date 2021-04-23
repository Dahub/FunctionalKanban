namespace FunctionalKanban.Domain.Project
{
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project.Events;

    public sealed record ProjectEntityState : State
    {
        public ProjectEntityState() => ProjectName = string.Empty;

        public string ProjectName { get; init; }

        public ProjectStatus ProjectStatus { get; init; }

        public bool IsDeleted { get; init; }

        protected override State With(Event @event) =>
            @event switch
            {
                ProjectCreated e    => this with { Version = e.EntityVersion, ProjectName = e.Name, ProjectStatus = e.Status, IsDeleted = e.IsDeleted },
                _                   => this with { }
            };
    }
}
