namespace FunctionalKanban.Domain.Common
{
    public abstract record State
    {
       public uint Version { get; init; }
    }
}
