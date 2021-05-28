namespace FunctionalKanban.Domain.Project.Events
{
    using FunctionalKanban.Domain.Common;

    public sealed record ProjectCreated : Event
    {
        public ProjectCreated() => Name = string.Empty;

        public string Name { get; init; }

        public ProjectStatus Status { get; init; }

        public bool IsDeleted { get; init; }
    }
}
