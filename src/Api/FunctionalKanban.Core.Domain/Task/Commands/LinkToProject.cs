namespace FunctionalKanban.Domain.Task.Commands
{
    using System;
    using FunctionalKanban.Domain.Common;

    public sealed record LinkToProject : Command
    {
        public Guid ProjectId { get; init; }
    }
}
