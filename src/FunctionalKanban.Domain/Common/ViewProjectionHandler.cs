namespace FunctionalKanban.Domain.Common
{
    using System;
    using FunctionalKanban.Functional;
    using Unit = System.ValueTuple;

    public interface IViewProjectionHandler
    {
        Exceptional<Unit> Handle(Event @event);
    }

    public abstract class ViewProjectionHandler<T> : IViewProjectionHandler where T : ViewProjection
    {
        protected readonly Func<Guid, Option<T>> _getViewProjectionById;

        protected readonly Func<T, Exceptional<Unit>> _upsertViewProjection;

        public ViewProjectionHandler(
           Func<Guid, Option<T>> getViewProjectionById,
           Func<T, Exceptional<Unit>> upsertViewProjection)
        {
            _getViewProjectionById = getViewProjectionById;
            _upsertViewProjection = upsertViewProjection;
        }
        public Exceptional<Unit> Handle(Event @event) =>
            CanHandle(@event)
            ? HandleEvent(_upsertViewProjection, _getViewProjectionById, @event)
            : Unit.Create();

        private Exceptional<Unit> HandleEvent(
               Func<T, Exceptional<Unit>> upsertViewProjection,
               Func<Guid, Option<T>> getViewProjectionById,
               Event @event) =>
           UpdateViewProjection(getViewProjectionById, @event).Match
           (
               Exception: (ex) => ex,
               Success: (p) => upsertViewProjection(p)
           );

        protected abstract Exceptional<T> UpdateViewProjection(
                Func<Guid, Option<T>> getViewProjectionById,
                Event @event);

        protected abstract bool CanHandle(Event @event);

        protected Exceptional<T> BuildError() => new Exception("Impossible de lire la projection");
    }
}
