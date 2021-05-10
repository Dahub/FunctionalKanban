namespace FunctionalKanban.Domain.Project.Events
{
    using FunctionalKanban.Domain.Common;

    public record ProjectDeleted : Event
    {
        public bool DeleteChlildrenTasks { get; init; }

        public bool IsDeleted { get; init; }
    }
}
