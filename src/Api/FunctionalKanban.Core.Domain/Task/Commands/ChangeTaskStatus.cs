namespace FunctionalKanban.Domain.Task.Commands
{
    using FunctionalKanban.Domain.Common;

    public sealed record ChangeTaskStatus : Command
    {
        public TaskStatus TaskStatus { get; init; }
    }
}
