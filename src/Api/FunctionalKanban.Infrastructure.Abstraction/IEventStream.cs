namespace FunctionalKanban.Infrastructure.Abstraction
{
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;
    using Unit = System.ValueTuple;

    public interface IEventStream
    {
        Exceptional<Unit> Push(Event @event);
    }
}
