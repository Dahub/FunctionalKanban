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
                Func<Guid, Exceptional<Validation<State>>> getEntity) =>
            ApplyCommandToEntities(command, getEntity).ToEvents();

        private static IEnumerable<Exceptional<Validation<EventAndState>>> ApplyCommandToEntities(
            LinkToProject command,
            Func<Guid, Exceptional<Validation<State>>> getEntity)
        {
            yield return getEntity(command.EntityId).ApplyCommand<TaskEntityState>(s => s.LinkToProject(command.TimeStamp, command.ProjectId));
            yield return getEntity(command.ProjectId).ApplyCommand<ProjectEntityState>(s => s.AddTaskToProject(command.TimeStamp, command.EntityId));
        }

        private static Exceptional<Validation<EventAndState>> ApplyCommand<T>(
                this Exceptional<Validation<State>> state, 
                Func<T, Validation<EventAndState>> f) where T : State =>
            state.Bind<Validation<State>, Validation<EventAndState>>(v => v.Bind(e => f((T)e)));

        private static Exceptional<Validation<IEnumerable<Event>>> ToEvents(
                this IEnumerable<Exceptional<Validation<EventAndState>>> eventsAndStates) =>
            eventsAndStates.ToMonadOfList().Bind(ConvertToExceptionalEvents);

        private static Exceptional<Validation<IEnumerable<Event>>> ConvertToExceptionalEvents(IEnumerable<Validation<EventAndState>> validations) => 
            Exceptional(validations.ToMonadOfList().Bind(ConvertToValidationEvents));

        private static Validation<IEnumerable<Event>> ConvertToValidationEvents(IEnumerable<EventAndState> eventsAndStates) =>
            Valid(eventsAndStates.Map(eas => eas.Event));
    }
}
