namespace FunctionalKanban.Core.Domain.Task.Commands
{
    using FunctionalKanban.Core.Domain.Common;

    public sealed record ChangeRemaningWork : Command
    {
        public uint RemaningWork { get; init; }
    }
}
