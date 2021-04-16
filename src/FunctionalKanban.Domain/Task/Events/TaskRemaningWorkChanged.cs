namespace FunctionalKanban.Domain.Task.Events
{
    using FunctionalKanban.Domain.Common;

    public sealed record TaskRemaningWorkChanged : Event
    {
        public uint RemaningWork { get; init; }

        public uint OldRemaningWork { get; init; }
    }
}
