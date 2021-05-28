namespace FunctionalKanban.Infrastructure.Implementation
{
    using System;
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;

    public class EventBus : IEventBus
    {
        private readonly Func<Event, Exceptional<Unit>> _streamEvent;

        private readonly Func<Event, Exceptional<Unit>> _notifySubscribers;

        public EventBus(
            Func<Event, Exceptional<Unit>> streamEvent,
            Func<Event, Exceptional<Unit>> notifySubscribers)
        {
            _streamEvent = streamEvent;
            _notifySubscribers = notifySubscribers;
        }

        public Exceptional<Unit> Publish(Event @event) =>
            _streamEvent(@event).Bind((_) => _notifySubscribers(@event));
    }
}
