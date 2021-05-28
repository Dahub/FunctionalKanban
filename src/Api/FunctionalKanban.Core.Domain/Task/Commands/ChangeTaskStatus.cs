namespace FunctionalKanban.Core.Domain.Task.Commands
{
    using FunctionalKanban.Core.Domain.Common;

    public sealed record ChangeTaskStatus : Command
    {
        public TaskStatus TaskStatus { get; init; }
    }
}
