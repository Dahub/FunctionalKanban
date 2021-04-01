﻿namespace FunctionalKanban.Domain.Task
{
    using FunctionalKanban.Domain.Common;

    public sealed record TaskState : State
    {
        public string TaskName { get; init; }

        public TaskStatus TaskStatus { get; init; }

        public uint RemaningWork { get; init; }
    }
}