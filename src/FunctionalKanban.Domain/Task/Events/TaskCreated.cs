namespace FunctionalKanban.Domain.Task.Events
{
    using FunctionalKanban.Domain.Common;

    public sealed record TaskCreated : Event
    {
        public string Name { get; init; }

        public TaskStatus Status { get; init; }

        public uint RemaningWork { get; init; }

        public bool IsDeleted { get; init; }
    }
}
