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
                      getEntity(command.EntityId).Bind<Validation<State>, Validation<EventAndState>>(v => v.Bind(e => ((TaskEntityState)e).LinkToProject(command))),
                      getEntity(command.ProjectId).Bind<Validation<State>, Validation<EventAndState>>(v => v.Bind(e => ((ProjectEntityState)e).AddTaskToProject(DateTime.Now, command.EntityId))));


        //public static Exceptional<Validation<IEnumerable<Event>>> HandleLinkToProjectCommand(
        //        LinkToProject command,
        //        Func<Guid, Exceptional<Validation<State>>> getEntity)
        //{

        //}

        private static Exceptional<Validation<IEnumerable<Event>>> ToEvents(
            params Exceptional<Validation<EventAndState>>[] eventsAndStates)
        {

            var exEnumValid = eventsAndStates.Aggregate(
                seed: Exceptional(Enumerable.Empty<Validation<EventAndState>>()),
                func: (list, next) => next.Match(
                    Exception: (ex) => new Exception("plop"),
                    Success: (value) => list.Bind((a) => Exceptional(a.Append(value)))));

        }


        //public static Exceptional<Validation<IEnumerable<Event>>> HandleLinkToProjectCommand(
        //      LinkToProject command,
        //      Func<Guid, Exceptional<Validation<State>>> getEntity)
        //  => getEntity(command.EntityId).
        //          Bind<Validation<State>, Validation<IEnumerable<Event>>>(v => v.
        //              Bind(e => ((TaskEntityState)e).LinkToProject(command).
        //                  Bind<EventAndState, IEnumerable<Event>>(eas => new List<Event>() { eas.Event })));

    }
}
