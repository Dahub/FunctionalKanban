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
        private readonly IViewProjectionRepository _viewProjectionRepository;

        public Notifier(IViewProjectionRepository viewProjectionRepository) => 
            _viewProjectionRepository = viewProjectionRepository;

        public Exceptional<Unit> Notity(Event @event) =>
            NotifyTaskViewProjection(_viewProjectionRepository, @event).
            Bind(_ => NotifyProjectViewProjection(_viewProjectionRepository, @event));

        private static Exceptional<Unit> NotifyTaskViewProjection(
                    IViewProjectionRepository repository,
                    Event @event) =>
            TaskViewProjection.CanHandle(@event)
            ? HandleEvent<TaskViewProjection>(repository, @event, () => new TaskViewProjection(), (p) => p.With(@event))
            : Unit.Create();

        private static Exceptional<Unit> NotifyProjectViewProjection(
                    IViewProjectionRepository repository,
                    Event @event) =>
            ProjectViewProjection.CanHandle(@event)
            ? HandleEvent<ProjectViewProjection>(repository, @event, () => new ProjectViewProjection(), (p) => p.With(@event))
            : Unit.Create();

        private static Exceptional<Unit> HandleEvent<T>(
                    IViewProjectionRepository repository,
                    Event @event,
                    Func<Exceptional<T>> create,
                    Func<T, T> update) where T : ViewProjection =>
            GetProjection(repository, @event.AggregateId, create).
            Bind((p) => repository.Upsert(update(p)));

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
