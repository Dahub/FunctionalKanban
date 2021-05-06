namespace FunctionalKanban.Service
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Shared;
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
    }
}
