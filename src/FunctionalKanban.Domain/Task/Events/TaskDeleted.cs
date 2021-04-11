namespace FunctionalKanban.Domain.Task.Events
{
    using FunctionalKanban.Domain.Common;

    public sealed record TaskDeleted : Event
    {
        public bool IsDeleted { get; init; }
    }
}
