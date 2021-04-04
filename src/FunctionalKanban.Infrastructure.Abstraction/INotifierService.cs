namespace FunctionalKanban.Infrastructure.Abstraction
{
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using Unit = System.ValueTuple;

    public interface INotifierService
    {
        Exceptional<Unit> Notify<T>(IViewProjectionRepository<T> viewProjectionRepository, Event @event) where T : ViewProjection;
    }
}
