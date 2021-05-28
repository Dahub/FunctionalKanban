namespace FunctionalKanban.Core.Domain.Task.Events
{
    public sealed record TaskStatusChanged : Common.Event
    {
        public TaskStatus NewStatus { get; init; }

        public uint RemaningWork { get; init; }
    }
}
