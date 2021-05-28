namespace FunctionalKanban.Core.Domain.Project.Commands
{
    using FunctionalKanban.Core.Domain.Common;

    public record DeleteProject : Command
    {
        public bool DeleteChildrenTasks { get; init; }
    }
}
