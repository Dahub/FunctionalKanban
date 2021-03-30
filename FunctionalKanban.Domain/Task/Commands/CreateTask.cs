namespace FunctionalKanban.Domain.Task.Commands
{
    using System;
    using FunctionalKanban.Domain.Common;

    public record CreateTask : Command
    {
        public string Name { get; init; }

        public uint RemaningWork { get; init; }
    }
}
