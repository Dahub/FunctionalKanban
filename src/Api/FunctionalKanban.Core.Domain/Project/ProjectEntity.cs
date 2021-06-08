namespace FunctionalKanban.Core.Domain.Project
{
    using System;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.Project.Commands;
    using FunctionalKanban.Core.Domain.Project.Events;
    using LaYumba.Functional;

    public static class ProjectEntity
    {
        public static Validation<EventAndState> Create(CreateProject cmd)
        {
            var @event = new ProjectCreated()
            {
                EntityId = cmd.EntityId,
                Name = cmd.Name,
                IsDeleted = false,
                TimeStamp = cmd.TimeStamp,
                Status = ProjectStatus.New
            };

            return new ProjectEntityState().ApplyEvent(@event);
        }

        public static Validation<EventAndState> AddTaskToProject(
            this ProjectEntityState state, 
            DateTime timeStamp,
            Guid taskId)
        {
            var @event = new ProjectNewTaskLinked()
            {
                EntityId = state.ProjectId,
                TaskId = taskId,
                TimeStamp = timeStamp
            };

            return state.ApplyEvent(@event);
        }

        public static Validation<EventAndState> Delete(
            this ProjectEntityState state, 
            DeleteProject cmd)
        {
            var @event = new ProjectDeleted()
            {
                EntityId = state.ProjectId,
                TimeStamp = cmd.TimeStamp,
                DeleteChildrenTasks = cmd.DeleteChildrenTasks,
                IsDeleted = true
            };

            return state.ApplyEvent(@event);
        }
    }
}
