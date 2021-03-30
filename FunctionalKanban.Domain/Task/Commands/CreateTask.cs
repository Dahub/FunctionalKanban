namespace FunctionalKanban.Domain.Task.Commands
{
    using FunctionalKanban.Domain.Common;

    public sealed record CreateTask : Command
    {
        public string Name { get; init; }

        public uint RemaningWork { get; init; }
    }
}
