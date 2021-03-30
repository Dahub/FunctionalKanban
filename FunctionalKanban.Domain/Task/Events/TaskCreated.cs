namespace FunctionalKanban.Domain.Task.Events
{
    public sealed record TaskCreated : Common.Event
    {
        public string Name { get; init; }

        public TaskStatus Status { get; init; }

        public uint RemaningWork { get; init; }
    }
}
