namespace FunctionalKanban.Infrastructure
{
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;

    public class EventBus : IEventBus
    {
        private readonly IEventStream _eventStream;

        private readonly INotifier _notifier;

        public EventBus(
            IEventStream eventStream,
            INotifier notifier)
        {
            _eventStream = eventStream;
            _notifier = notifier;
        }

        public Exceptional<Unit> Publish(Event @event) =>
            _eventStream.Push(@event)
                .Bind((_) => _notifier.Notity(@event));
    }
}
