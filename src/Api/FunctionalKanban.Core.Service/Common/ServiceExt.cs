namespace FunctionalKanban.Core.Service.Common
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Shared;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    public static class ServiceExt
    {
        internal static Exceptional<Validation<IEnumerable<Event>>> ToEvents(
                  this IEnumerable<Exceptional<Validation<EventAndState>>> eventsAndStates) =>
              eventsAndStates.ToMonadOfList().Bind(ConvertToExceptionalEvents);

        private static Exceptional<Validation<IEnumerable<Event>>> ConvertToExceptionalEvents(IEnumerable<Validation<EventAndState>> validations) =>
            Exceptional(validations.ToMonadOfList().Bind(ConvertToValidationEvents));

        private static Validation<IEnumerable<Event>> ConvertToValidationEvents(IEnumerable<EventAndState> eventsAndStates) =>
            Valid(eventsAndStates.Map(eas => eas.Event));

        internal static Exceptional<Validation<EventAndState>> ApplyCommand<T>(
                this Exceptional<Validation<State>> state,
                Func<T, Validation<EventAndState>> f) where T : State =>
            state.Bind<Validation<State>, Validation<EventAndState>>(v => v.Bind(e => f((T)e)));
    }
}
