namespace FunctionalKanban.Service
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;
    using Unit = System.ValueTuple;

    public static class TaskAndProjectLinkService
    {
        public static Exceptional<Validation<IEnumerable<Event>>> HandleLinkToProjectCommand(
                LinkToProject command,
                Func<Guid, Exceptional<Validation<State>>> getEntity)
            => ToEvents(
                NewMethod(command, getEntity),
                getEntity(command.ProjectId).Bind((Func<Validation<State>, Exceptional<Validation<EventAndState>>>)(v => v.Bind(e => ((ProjectEntityState)e).AddTaskToProject(DateTime.Now, command.EntityId)))));
        private static Exceptional<Validation<EventAndState>> NewMethod(LinkToProject command, Func<Guid, Exceptional<Validation<State>>> getEntity) => getEntity(command.EntityId).Bind<Validation<State>, Validation<EventAndState>>(v => v.Bind(e => ((TaskEntityState)e).LinkToProject(command)));

        private static Exceptional<Validation<IEnumerable<Event>>> ToEvents(params Exceptional<Validation<EventAndState>>[] eventsAndStates)
        {

        }

        private static Exceptional<Validation<IEnumerable<Event>>> AddEvent(
            this Exceptional<Validation<IEnumerable<Event>>> events,
            Exceptional<Validation<EventAndState>> @event) =>


    }
}
