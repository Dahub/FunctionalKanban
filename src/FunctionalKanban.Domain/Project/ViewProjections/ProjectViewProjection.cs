namespace FunctionalKanban.Domain.Project.ViewProjections
{
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Project.Events;

    public record ProjectViewProjection : ViewProjection
    {
        public ProjectViewProjection() => Name = string.Empty;

        public string Name { get; init; }

        public ProjectStatus Status { get; init; }

        public bool IsDeleted { get; init; }

        public uint TotalRemaningWork { get; init; }

        public static bool CanHandle(Event @event) =>
            @event is ProjectCreated;
    }

    public static class ProjectViewProjectionExt
    {
        public static ProjectViewProjection With(this ProjectViewProjection view, Event @event) =>
            @event switch
            {
                ProjectCreated e => view with { Id = e.AggregateId, Name = e.Name, Status = e.Status, IsDeleted = e.IsDeleted, TotalRemaningWork = 0 },
                 _ => view with { }
            };
    }
}
