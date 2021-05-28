namespace FunctionalKanban.Domain.Project.Events
{
    using FunctionalKanban.Domain.Common;

    public record ProjectDeleted : Event
    {
        public bool DeleteChildrenTasks { get; init; }

        public bool IsDeleted { get; init; }
    }
}
