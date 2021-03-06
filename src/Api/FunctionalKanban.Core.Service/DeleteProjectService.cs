namespace FunctionalKanban.Core.Service
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.Project;
    using FunctionalKanban.Core.Domain.Task;
    using FunctionalKanban.Core.Domain.Project.Commands;
    using FunctionalKanban.Core.Shared;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;
    using FunctionalKanban.Core.Service.Common;
    using System.Linq;

    public static class DeleteProjectService
    {
        public static Exceptional<Validation<IEnumerable<Event>>> HandleDeleteProjectCommand(
                  DeleteProject command,
                  Func<Guid, Exceptional<Validation<State>>> getEntity) =>
            ApplyCommandToEntities(command, getEntity).ToEvents();

        private static IEnumerable<Exceptional<Validation<EventAndState>>> ApplyCommandToEntities(
                DeleteProject command,
                Func<Guid, Exceptional<Validation<State>>> getEntity) =>
            getEntity(command.EntityId).
            Bind<Validation<State>, Validation<ProjectEntityState>>(x => x.CastTo<State, ProjectEntityState>()).
            Match(
                Exception:  (ex)    => LaYumba.Functional.Exceptional.Of<Validation<EventAndState>>(ex).ToEnumerable(), 
                Success:    (v)     => v.Match(
                    Invalid: (errors)   => errors.Select(e => LaYumba.Functional.Exceptional.Of<Validation<EventAndState>>(new Exception(e.Message))),
                    Valid:   (p)        => p.AggregateEvents(command, getEntity)));

        private static IEnumerable<Exceptional<Validation<EventAndState>>> AggregateEvents(
                this ProjectEntityState p,
                DeleteProject cmd,
                Func<Guid, Exceptional<Validation<State>>> getEntity) =>
            p.AssociatedTaskIds.Aggregate(
                seed: Exceptional(p.Delete(cmd)).ToEnumerable(),
                func: (list, taskId) => list.Append(
                    getEntity(taskId).Bind<Validation<State>, Validation<EventAndState>>
                        (e => e.CastTo<State, TaskEntityState>().Bind((v) =>
                            cmd.DeleteChildrenTasks
                                ? v.Delete(cmd.TimeStamp)
                                : v.RemoveFromProject(cmd.TimeStamp, cmd.EntityId)))));

        private static IEnumerable<Exceptional<T>> ToEnumerable<T>(this Exceptional<T> ex) 
        {
            if (ex.Success)
            {
                yield return ex;
            }
        }
    }
}
