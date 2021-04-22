namespace FunctionalKanban.Domain.Task.Commands
{
    using FunctionalKanban.Domain.Common;

    public sealed record ChangeRemaningWork : Command
    {
        public uint RemaningWork { get; init; }
    }
}
