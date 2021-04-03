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
            history.OrderBy(h => h.EntityVersion).AreConsecutives()
                ? Hydrate<TState, TCreatedEvent>(history, createState, applyEvent)
                : None;

        private static bool AreConsecutives(this IEnumerable<Event> events) =>
            !events.Select(e => e.EntityVersion).Select((i, j) => i - j).Distinct().Skip(1).Any();

        private static Option<TState> Hydrate<TState, TCreatedEvent>(
                   IEnumerable<Event> history,
                   Func<TState> createState,
                   Func<TState, Event, Validation<TState>> applyEvent)
               where TState : State
               where TCreatedEvent : Event =>
            history.OrderBy(h => h.EntityVersion)
                .Match(
                    Empty: () => None,
                    Otherwise: (createdEvent, otherEvents) =>
                       otherEvents.Aggregate(
                           seed: applyEvent(createState(), (TCreatedEvent)createdEvent),
                           func: (state, evt) => state.Bind(s => applyEvent(s, evt)))
                    .Match(
                        Invalid: (_) => None,
                        Valid: (state) => Some(state)));
    }
}
