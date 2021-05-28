namespace FunctionalKanban.Core.Domain.Task.Commands
{
    using FunctionalKanban.Core.Domain.Common;

    public sealed record CreateTask : Command
    {
        public CreateTask() => Name = string.Empty;

        public string Name { get; init; }

        public uint RemaningWork { get; init; }
    }
}
