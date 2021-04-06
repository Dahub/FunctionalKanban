namespace FunctionalKanban.Domain.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    internal static class EntityStateHelper
    {
        public static Validation<EventAndState> ToEventAndState(this Validation<State> state, Event @event) =>
            state.Bind<State, EventAndState>((s) => new EventAndState(@event, s));

        public static Option<State> From<TCreatedEvent>(
             IEnumerable<Event> history,
             Func<State> createState,
             Func<State, Event, Validation<State>> applyEvent)
                  where TCreatedEvent : Event =>
            FromOrdered<TCreatedEvent>(
                history.OrderBy(h => h.EntityVersion), 
                createState, 
                applyEvent);

        private static Option<State> FromOrdered<TCreatedEvent>(
             IEnumerable<Event> history,
             Func<State> createState,
             Func<State, Event, Validation<State>> applyEvent)
                  where TCreatedEvent : Event =>
            history.IsValid<TCreatedEvent>()
                ? Hydrate<TCreatedEvent>(history, createState, applyEvent).ToOption()
                : None;

        private static bool IsValid<TCreatedEvent>(this IEnumerable<Event> events) =>
            events.IsNotNullOrEmpty()
            && events.StartByCreatedEvent<TCreatedEvent>()
            && events.AreConsecutives();

        private static bool IsNotNullOrEmpty(this IEnumerable<Event> events) =>
            events != null && events.Any();

        private static bool StartByCreatedEvent<TCreatedEvent>(this IEnumerable<Event> events) =>
            events.Any() && events.First() is TCreatedEvent;

        private static bool AreConsecutives(this IEnumerable<Event> events) =>
            !events.Select(e => e.EntityVersion).Select((i, j) => i - j).Distinct().Skip(1).Any();

        private static Validation<State> Hydrate<TCreatedEvent>(
            this IEnumerable<Event> orderedEvents,
            Func<State> createState,
            Func<State, Event, Validation<State>> applyEvent)
                    where TCreatedEvent : Event =>
                orderedEvents.Skip(1).Aggregate(
                    seed: applyEvent(createState(), orderedEvents.First()),
                    func: (state, evt) => state.Bind(s => applyEvent(s, evt)));

        private static Option<State> ToOption(this Validation<State> validation) =>
            validation.Match(
                Invalid: (_) => None,
                Valid: (state) => Some(state));
    }
}
