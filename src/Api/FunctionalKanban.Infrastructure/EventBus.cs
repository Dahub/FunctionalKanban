namespace FunctionalKanban.Infrastructure
{
    using System;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using static LaYumba.Functional.F;
    using Unit = System.ValueTuple;

    public class EventBus : IEventBus
    {
        private readonly Func<Event[], Exceptional<Unit>> _streamEvent;

        private readonly Func<Event, Exceptional<Unit>> _notifySubscribers;

        public EventBus(
            Func<Event[], Exceptional<Unit>> streamEvent,
            Func<Event, Exceptional<Unit>> notifySubscribers)
        {
            _streamEvent = streamEvent;
            _notifySubscribers = notifySubscribers;
        }

        public Exceptional<Unit> Publish(params Event[] events) =>
            _streamEvent(events).Bind((_) =>
                events.Aggregate(
                    seed: Exceptional(Unit.Create()),
                    func: (ex, @event) => ex.Bind(_ => _notifySubscribers(@event))));
    }
}
