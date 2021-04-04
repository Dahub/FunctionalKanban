namespace FunctionalKanban.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;

    public class EventBus : IEventBus
    {
        private readonly IEventStream _eventStream;

        private readonly IList<IViewProjectionHandler> _subscribers = new List<IViewProjectionHandler>();

        public EventBus(
            IEventStream eventStream) => _eventStream = eventStream;

        public Exceptional<Unit> Publish(Event @event) =>
            _eventStream.Push(@event).Match(
                Exception: (ex) => ex,
                Success: (_) => PublishToSubscribers(_subscribers, @event));

        private Exceptional<Unit> PublishToSubscribers(IList<IViewProjectionHandler> subscribers, Event @event)
        {
            var errors = subscribers.Map((s) => s.Handle(@event)).Where(e => e.Exception);
            return errors.Any()
                ? errors.First()
                : Unit.Create();
        }            

        public Unit Subscribe(IViewProjectionHandler viewProjectionHandler) =>
            _subscribers.Contains(viewProjectionHandler)
            ? AddSubscriber(viewProjectionHandler)
            : Unit.Create();

        private Unit AddSubscriber(IViewProjectionHandler subscriber) 
        { 
            _subscribers.Add(subscriber);
            return Unit.Create();
        }
    }
}
