namespace FunctionalKanban.Infrastructure.Abstraction
{
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using Unit = System.ValueTuple;

    public interface IEventStream
    {
        Exceptional<Unit> Push(Event @event);
    }
}
