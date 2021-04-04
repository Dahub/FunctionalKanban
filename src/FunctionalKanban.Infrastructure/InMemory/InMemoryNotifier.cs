namespace FunctionalKanban.Infrastructure.InMemory
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;

    public class InMemoryNotifier : INotifier
    {
        private readonly InMemoryViewProjectionRepository<TaskViewProjection> _taskViewProjectionRepository;

        public InMemoryNotifier(IInMemoryDatabase database)
        {
            _taskViewProjectionRepository = new InMemoryViewProjectionRepository<TaskViewProjection>(database);
        }

        public Exceptional<Unit> Notity(Event @event) =>
            NotifyTaskViewProjection(_taskViewProjectionRepository, @event); // bind others notify

        private Exceptional<Unit> NotifyTaskViewProjection(
                    InMemoryViewProjectionRepository<TaskViewProjection> repository,
                    Event @event) =>
            TaskViewProjection.CanHandle(@event)
            ? HandleEvent(repository, @event, () => new TaskViewProjection(), (p) => p.With(@event))
            : Unit.Create();

        private Exceptional<Unit> HandleEvent<T>(
                    InMemoryViewProjectionRepository<T> repository,
                    Event @event,
                    Func<Exceptional<T>> buildNewViewProjection,
                    Func<T, T> updateViewProjection) where T : ViewProjection =>
            GetProjection(repository, @event.AggregateId, buildNewViewProjection)
            .Bind((p) => repository.Upsert(updateViewProjection(p)));

        private Exceptional<T> GetProjection<T>(
                    InMemoryViewProjectionRepository<T> repository,
                    Guid id,
                    Func<Exceptional<T>> buildNewViewProjection) where T : ViewProjection => 
            repository.GetById(id).Match(
                Exception: (ex) => Exceptional.Of<T>(ex),
                Success: (p)    => p.Match(
                        None: ()    => buildNewViewProjection(),
                        Some: (p)   => Exceptional.Of(p)));
    }
}
