namespace FunctionalKanban.Service
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Service.Common;
    using LaYumba.Functional;

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
    }
}
