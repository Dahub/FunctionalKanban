namespace FunctionalKanban.Infrastructure
{
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Infrastructure.Abstraction;
    using FunctionalKanban.Functional;
    using Unit = System.ValueTuple;

    public class EventBus : IEventBus
    {
        private readonly IEventStream _eventStream;

        public EventBus(IEventStream eventStream) => _eventStream = eventStream;

        public Exceptional<Unit> Publish(Event @event) => _eventStream.Push(@event);
    }
}
