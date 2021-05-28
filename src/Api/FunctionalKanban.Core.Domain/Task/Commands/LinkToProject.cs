namespace FunctionalKanban.Core.Domain.Task.Commands
{
    using System;
    using FunctionalKanban.Core.Domain.Common;

    public sealed record LinkToProject : Command
    {
        public Guid ProjectId { get; init; }
    }
}
