namespace FunctionalKanban.Domain.Common
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    public abstract record State
    {
        public uint Version { get; init; }

        public Validation<EventAndState> ApplyEvent(Event @event) => 
            @event.EntityVersion != Version + 1 
            ? Invalid($"Version d'événement {@event.EntityVersion} incorrecte, attendue {Version + 1}")
            : new EventAndState(@event, With(@event));

        protected abstract State With(Event @event);

        public Option<State> From(
            IEnumerable<Event> history) =>
                OrderEvents(history).
                    Bind(HistoryIsValid).
                    Bind((evts) => Some(Hydrate(evts, this, (state, evt) => state.With(evt))));

        private static Option<IEnumerable<Event>> OrderEvents(IEnumerable<Event> events) =>
            Some(events.OrderBy(e => e.EntityVersion).AsEnumerable());

        private static Option<IEnumerable<Event>> HistoryIsValid(IEnumerable<Event> events) => 
            events.Any() 
            && AreConsecutives(events) 
            && AreSameEntity(events)
                ? Some(events)
                : None;

        private static bool AreConsecutives(IEnumerable<Event> events) =>
            !events.Map(e => e.EntityVersion).Select((i, j) => i - j).Distinct().Skip(1).Any();

        private static bool AreSameEntity(IEnumerable<Event> events) =>
            !events.Map(e => e.EntityId).Distinct().Skip(1).Any();

        private static State Hydrate(
            IEnumerable<Event> orderedEvents,
            State initialState,
            Func<State, Event, State> applyEvent) =>
                orderedEvents.Skip(1).Aggregate(
                    seed: applyEvent(initialState, orderedEvents.First()),
                    func: (state, evt) => applyEvent(state, evt));
    }
}
