namespace FunctionalKanban.Domain.Task.Commands
{
    using System;
    using FunctionalKanban.Domain.Common;

    public sealed record CreateTask : Command
    {
        public CreateTask() => Name = string.Empty;

        public string Name { get; init; }

        public uint RemaningWork { get; init; }

        public Guid? ProjectId { get; init; }
    }
}
