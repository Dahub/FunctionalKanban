namespace FunctionalKanban.Core.Domain.Project.Events
{
    using FunctionalKanban.Core.Domain.Common;

    public record ProjectDeleted : Event
    {
        public bool DeleteChildrenTasks { get; init; }

        public bool IsDeleted { get; init; }
    }
}
