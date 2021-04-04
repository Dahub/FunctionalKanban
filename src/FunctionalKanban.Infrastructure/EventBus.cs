namespace FunctionalKanban.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.ViewProjections;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;

    public class EventBus : IEventBus
    {
        private readonly IEventStream _eventStream;

        private readonly List<Type> _subscribers = new List<Type>()
        {
            typeof(TaskViewProjection)
        };

        public EventBus(
            IEventStream eventStream) => _eventStream = eventStream;

        public Exceptional<Unit> Publish(Event @event) => _eventStream.Push(@event);
    }
}
