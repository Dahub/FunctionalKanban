namespace FunctionalKanban.Infrastructure
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project.ViewProjections;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;

    public class Notifier : INotifier
    {
        private readonly IViewProjectionRepository _repo;

        public Notifier(IViewProjectionRepository viewProjectionRepository) => _repo = viewProjectionRepository;

        public Exceptional<Unit> Notify(Event @event) =>
            Notify<TaskViewProjection>(_repo, @event, TaskViewProjection.CanHandle).
            Bind(_ => Notify<ProjectViewProjection>(_repo, @event, ProjectViewProjection.CanHandle));

       private static Exceptional<Unit> Notify<T>(
                    IViewProjectionRepository repository,
                    Event @event,
                    Func<Event, bool> canHandle) where T : ViewProjection, new() =>
            canHandle(@event)
            ? HandleEvent<T>(repository, @event, () => new T())
            : Unit.Create();

        private static Exceptional<Unit> HandleEvent<T>(
                    IViewProjectionRepository repository,
                    Event @event,
                    Func<Exceptional<T>> create) where T : ViewProjection =>
            GetProjection(repository, @event.EntityId, create).
            Bind((p) => repository.Upsert(p.With(@event)));

        private static Exceptional<T> GetProjection<T>(
                    IViewProjectionRepository repository,
                    Guid id,
                    Func<Exceptional<T>> buildNewViewProjection) where T : ViewProjection =>
            repository.GetById<T>(id).Match(
                Exception: (ex) => Exceptional.Of<T>(ex),
                Success: (p) => p.Match(
                        None: () => buildNewViewProjection(),
                        Some: (p) => Exceptional.Of(p)));
    }
}
