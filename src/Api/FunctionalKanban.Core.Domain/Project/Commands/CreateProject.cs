namespace FunctionalKanban.Domain.Project.Commands
{
    using FunctionalKanban.Domain.Common;

    public sealed record CreateProject : Command
    {
        public CreateProject() => Name = string.Empty;

        public string Name { get; init; }
    }
}
