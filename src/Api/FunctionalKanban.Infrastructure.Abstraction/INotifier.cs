namespace FunctionalKanban.Infrastructure.Abstraction
{
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;
    using Unit = System.ValueTuple;

    public interface INotifier
    {
        Exceptional<Unit> Notify(Event @event);
    }
}
