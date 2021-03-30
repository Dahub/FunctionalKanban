namespace FunctionalKanban.Domain.Task.Commands
{
    using System;
    using FunctionalKanban.Domain.Common;

    public record ChangeTaskStatus : Command
    {
        public Guid TaskId { get; init; }

        public TaskStatus TaskStatus { get; init; }
    }
}
