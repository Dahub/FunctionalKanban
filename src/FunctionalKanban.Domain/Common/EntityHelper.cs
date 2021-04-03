namespace FunctionalKanban.Domain.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    internal static class EntityHelper
    {
        public static Validation<EventAndState> ToEventAndState<T>(this Validation<T> state, Event @event) where T : State =>
            state.Bind<T, EventAndState>((s) => new EventAndState(@event, s));

        public static Option<TState> From<TState, TCreatedEvent>(
             IEnumerable<Event> history,
             Func<TState> createState,
             Func<TState, Event, Validation<TState>> applyEvent)
                  where TState : State
                  where TCreatedEvent : Event =>
            FromOrdered<TState, TCreatedEvent>(
                history.OrderBy(h => h.EntityVersion), 
                createState, 
                applyEvent);

        private static Option<TState> FromOrdered<TState, TCreatedEvent>(
             IEnumerable<Event> history,
             Func<TState> createState,
             Func<TState, Event, Validation<TState>> applyEvent)
                  where TState : State
                  where TCreatedEvent : Event =>
            history.IsValid<TCreatedEvent>()
                ? Hydrate<TState, TCreatedEvent>(history, createState, applyEvent).ToOption()
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

        private static Validation<TState> Hydrate<TState, TCreatedEvent>(
            this IEnumerable<Event> orderedEvents,
            Func<TState> createState,
            Func<TState, Event, Validation<TState>> applyEvent)
                    where TState : State
                    where TCreatedEvent : Event =>
                orderedEvents.Skip(1).Aggregate(
                    seed: applyEvent(createState(), orderedEvents.First()),
                    func: (state, evt) => state.Bind(s => applyEvent(s, evt)));

        private static Option<TState> ToOption<TState>(this Validation<TState> validation) =>
            validation.Match(
                Invalid: (_) => None,
                Valid: (state) => Some(state));
    }
}
