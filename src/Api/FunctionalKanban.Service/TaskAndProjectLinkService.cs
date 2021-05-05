namespace FunctionalKanban.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    public static class TaskAndProjectLinkService
    {
        public static Exceptional<Validation<IEnumerable<Event>>> HandleLinkToProjectCommand(
              LinkToProject command,
              Func<Guid, Exceptional<Validation<State>>> getEntity)
                  => ToEvents(
                      getEntity(command.EntityId).Bind<Validation<State>, Validation<EventAndState>>(v => v.Bind(e => ((TaskEntityState)e).LinkToProject(command.TimeStamp, command.ProjectId))),
                      getEntity(command.ProjectId).Bind<Validation<State>, Validation<EventAndState>>(v => v.Bind(e => ((ProjectEntityState)e).AddTaskToProject(command.TimeStamp, command.EntityId))));

        private static Exceptional<Validation<IEnumerable<Event>>> ToEvents(
                params Exceptional<Validation<EventAndState>>[] eventsAndStates) =>
            eventsAndStates.ToExceptionalOfList().Bind(ConvertToExceptionalEvents);

        private static Exceptional<Validation<IEnumerable<Event>>> ConvertToExceptionalEvents(IEnumerable<Validation<EventAndState>> validations) => 
            Exceptional(validations.ToValidationOfList().Bind(ConvertToValidationEvents));

        private static Validation<IEnumerable<Event>> ConvertToValidationEvents(IEnumerable<EventAndState> eventsAndStates) =>
            Valid(eventsAndStates.Map(eas => eas.Event));

        private static Exceptional<IEnumerable<T>> ToExceptionalOfList<T>(this IEnumerable<Exceptional<T>> exceptionals) =>
            exceptionals.Aggregate(
                seed: Exceptional(Enumerable.Empty<T>()),
                func: (list, next) => next.Match(
                    Exception:  (ex)    => ex,
                    Success:    (value) => list.Bind((a) => Exceptional(a.Append(value)))));

        private static Validation<IEnumerable<T>> ToValidationOfList<T>(this IEnumerable<Validation<T>> exceptionals) =>
            exceptionals.Aggregate(
                seed: Valid(Enumerable.Empty<T>()),
                func: (list, next) => next.Match(
                    Invalid:    (errors)    => Invalid(errors),
                    Valid:      (value)     => list.Bind((a) => Valid(a.Append(value)))));
    }
}
