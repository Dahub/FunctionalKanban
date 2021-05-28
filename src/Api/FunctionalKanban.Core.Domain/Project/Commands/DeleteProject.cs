namespace FunctionalKanban.Domain.Project.Commands
{
    using FunctionalKanban.Domain.Common;

    public record DeleteProject : Command
    {
        public bool DeleteChildrenTasks { get; init; }
    }
}
