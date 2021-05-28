namespace FunctionalKanban.Domain.Project
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project.Commands;
    using FunctionalKanban.Domain.Project.Events;
    using LaYumba.Functional;

    public static class ProjectEntity
    {
        private static readonly string _entityName = typeof(ProjectEntityState).FullName ?? string.Empty;

        public static Validation<EventAndState> Create(CreateProject cmd)
        {
            var @event = new ProjectCreated()
            {
                EntityId = cmd.EntityId,
                EntityName = _entityName,
                Name = cmd.Name,
                IsDeleted = false,
                TimeStamp = cmd.TimeStamp,
                Status = ProjectStatus.New,
                EntityVersion = 1
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
                EntityName = _entityName,
                EntityVersion = state.Version + 1,
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
                EntityName = _entityName,
                EntityVersion = state.Version + 1,
                TimeStamp = cmd.TimeStamp,
                DeleteChildrenTasks = cmd.DeleteChildrenTasks,
                IsDeleted = true
            };

            return state.ApplyEvent(@event);
        }
    }
}
