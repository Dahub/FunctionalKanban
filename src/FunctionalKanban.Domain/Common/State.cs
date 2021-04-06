namespace FunctionalKanban.Domain.Common
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    public abstract record State
    {
        public uint Version { get; init; }

        public abstract Option<State> From(IEnumerable<Event> history);

        protected Option<State> From<T>(
            IEnumerable<Event> history,
            Func<State> createState) where T : Event =>
                OrderEvents(history)
                    .Bind(HistoryIsValid<T>())
                    .Match(
                        None: () => None,
                        Some: (evts) => ConvertToOption( Hydrate(evts, createState, (state, evt) => ApplyEvent(evt))));

        public abstract Validation<State> ApplyEvent(Event @event);

        public Validation<EventAndState> Mutate(Event @event) =>
            ApplyEvent(@event).Bind<State, EventAndState>((s) => new EventAndState(@event, s));

        private static Option<IEnumerable<Event>> OrderEvents(IEnumerable<Event> events) =>
            Some(events.OrderBy(e => e.EntityVersion).AsEnumerable());

        private static Func<IEnumerable<Event>, Option<IEnumerable<Event>>> HistoryIsValid<T>() where T : Event =>
            (events) => events.Any() && AreConsecutives(events) && events.First() is T
                ? Some(events)
                : None;

        private static bool AreConsecutives(IEnumerable<Event> events) =>
            !events.Select(e => e.EntityVersion).Select((i, j) => i - j).Distinct().Skip(1).Any();

        private static Validation<State> Hydrate(
            IEnumerable<Event> orderedEvents,
            Func<State> createState,
            Func<State, Event, Validation<State>> applyEvent) =>
                orderedEvents.Skip(1).Aggregate(
                    seed: applyEvent(createState(), orderedEvents.First()),
                    func: (state, evt) => state.Bind(s => applyEvent(s, evt)));

        private static Option<State> ConvertToOption(Validation<State> validation) =>
           validation.Match(
               Invalid: (_) => None,
               Valid: (state) => Some(state));
    }
}
