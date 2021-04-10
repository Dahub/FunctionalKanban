namespace FunctionalKanban.Infrastructure
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;

    public class Notifier : INotifier
    {
        private readonly IViewProjectionRepository<TaskViewProjection> _taskViewProjectionRepository;

        public Notifier(IViewProjectionRepository<TaskViewProjection> taskViewProjectionRepository)
        {
            _taskViewProjectionRepository = taskViewProjectionRepository;
        }

        public Exceptional<Unit> Notity(Event @event) =>
            NotifyTaskViewProjection(_taskViewProjectionRepository, @event); // bind others notify

        private static Exceptional<Unit> NotifyTaskViewProjection(
                    IViewProjectionRepository<TaskViewProjection> repository,
                    Event @event) =>
            TaskViewProjection.CanHandle(@event)
            ? HandleEvent(repository, @event, () => new TaskViewProjection(), (p) => p.With(@event))
            : Unit.Create();

        private static Exceptional<Unit> HandleEvent<T>(
                    IViewProjectionRepository<T> repository,
                    Event @event,
                    Func<Exceptional<T>> create,
                    Func<T, T> update) where T : ViewProjection =>
            GetProjection(repository, @event.AggregateId, create).
            Bind((p) => repository.Upsert(update(p)));

        private static Exceptional<T> GetProjection<T>(
                    IViewProjectionRepository<T> repository,
                    Guid id,
                    Func<Exceptional<T>> buildNewViewProjection) where T : ViewProjection => 
            repository.GetById(id).Match(
                Exception: (ex) => Exceptional.Of<T>(ex),
                Success: (p)    => p.Match(
                        None: ()    => buildNewViewProjection(),
                        Some: (p)   => Exceptional.Of(p)));
    }
}
