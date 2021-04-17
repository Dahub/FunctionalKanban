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

        public abstract Option<State> From(IEnumerable<Event> history);

        public Validation<EventAndState> ApplyEvent(Event @event) => new EventAndState(@event, With(@event));

        protected abstract State With(Event @event);

        protected Option<State> From<T>(
            IEnumerable<Event> history,
            Func<State> createState) where T : Event =>
                OrderEvents(history).
                    Bind(HistoryIsValid<T>()).
                    Match(
                        None: ()        => None,
                        Some: (evts)    => Some(Hydrate(evts, createState(), (state, evt) => With(evt))));

        private static Option<IEnumerable<Event>> OrderEvents(IEnumerable<Event> events) =>
            Some(events.OrderBy(e => e.EntityVersion).AsEnumerable());

        private static Func<IEnumerable<Event>, Option<IEnumerable<Event>>> HistoryIsValid<T>() where T : Event => (events) => 
            events.Any() 
            && AreConsecutives(events) 
            && AreSameEntity(events)
            && events.First() is T
                ? Some(events)
                : None;

        private static bool AreConsecutives(IEnumerable<Event> events) =>
            !events.Map(e => e.EntityVersion). Select((i, j) => i - j).Distinct().Skip(1).Any();

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
