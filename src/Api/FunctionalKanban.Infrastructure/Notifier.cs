namespace FunctionalKanban.Infrastructure
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.ViewProjections;
    using LaYumba.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;

    public class Notifier : INotifier
    {
        private readonly IViewProjectionRepository _repo;

        public Notifier(IViewProjectionRepository viewProjectionRepository) => _repo = viewProjectionRepository;

        public Exceptional<Unit> Notify(Event @event) =>
            Notify<TaskViewProjection>(_repo, @event, TaskViewProjection.HandleWithId).
            Bind(_ => Notify<ProjectViewProjection>(_repo, @event, ProjectViewProjection.HandleWithId));

        private static Exceptional<Unit> Notify<T>(
                     IViewProjectionRepository repository,
                     Event @event,
                     Func<Event, Option<Guid>> handleWithId) where T : ViewProjection, new() =>
             handleWithId(@event).Match(
                 None: ()   => Unit.Create(),
                 Some: (id) => HandleEvent<T>(repository, id, @event, () => new T()));

        private static Exceptional<Unit> HandleEvent<T>(
                    IViewProjectionRepository repository,
                    Guid projectionId, 
                    Event @event,
                    Func<Exceptional<T>> create) where T : ViewProjection =>
            GetProjection(repository, projectionId, create).
            Bind((p) => p.With(@event).Match(
                Some: (value)   => repository.Upsert(value),
                None: ()        => repository.Delete(p)));

        private static Exceptional<T> GetProjection<T>(
                    IViewProjectionRepository repository,
                    Guid id,
                    Func<Exceptional<T>> createdNewViewProjection) where T : ViewProjection =>
            repository.GetById<T>(id).Match(
                Exception:  (e) => Exceptional.Of<T>(e),
                Success:    (p) => p.Match(
                        None: ()    => createdNewViewProjection(),
                        Some: (p)   => Exceptional.Of(p)));
    }
}
