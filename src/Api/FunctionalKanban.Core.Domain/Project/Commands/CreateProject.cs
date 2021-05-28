namespace FunctionalKanban.Core.Domain.Project.Commands
{
    using FunctionalKanban.Core.Domain.Common;

    public sealed record CreateProject : Command
    {
        public CreateProject() => Name = string.Empty;

        public string Name { get; init; }
    }
}
