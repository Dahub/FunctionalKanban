namespace FunctionalKanban.Infrastructure.InMemory
{
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

        public Exceptional<Unit> Notity(Event @event)
        {
            if(TaskViewProjection.CanHandle(@event))
            {
                var entity = _taskViewProjectionRepository.GetById(@event.AggregateId).Match(
                    None: () => new TaskViewProjection(),
                    Some: (p) => p);

                return _taskViewProjectionRepository.Upsert(entity.With(@event));
            }

            return Unit.Create();
        }
    }
}
