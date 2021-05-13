namespace FunctionalKanban.Domain.Task
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Events;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    public static class TaskEntity
    {
        private static readonly string _entityName = typeof(TaskEntityState).FullName ?? string.Empty;

        public static Validation<EventAndState> Create(CreateTask cmd)
        {
            var @event = new TaskCreated()
            {
                EntityId        = cmd.EntityId,
                EntityName      = _entityName,
                Name            = cmd.Name,
                RemaningWork    = cmd.RemaningWork,
                IsDeleted       = false,
                TimeStamp       = cmd.TimeStamp,
                Status          = TaskStatus.Todo,
                EntityVersion   = 1,
                ProjectId       = None
            };

            return new TaskEntityState().ApplyEvent(@event);
        }

        public static Validation<EventAndState> ChangeStatus(
                this TaskEntityState state,
                ChangeTaskStatus cmd)
        {
            var @event = new TaskStatusChanged()
            {
                EntityId     = cmd.EntityId,
                EntityName   = _entityName,
                EntityVersion   = state.Version + 1,
                NewStatus       = cmd.TaskStatus,
                TimeStamp       = cmd.TimeStamp,
                RemaningWork    = cmd.TaskStatus.Equals(
                                    TaskStatus.Done |
                                    TaskStatus.Canceled |
                                    TaskStatus.Archived) ? 0 : state.RemaningWork
            };

            return state.WithCheckNotDeleted().Bind(s => s.ApplyEvent(@event));
        }

        public static Validation<EventAndState> Delete(
                this TaskEntityState state,
                DeleteTask cmd) => state.Delete(cmd.TimeStamp);

        public static Validation<EventAndState> Delete(
                this TaskEntityState state,
                DateTime timeStamp)
        {
            var @event = new TaskDeleted()
            {
                EntityId = state.TaskId,
                EntityName = _entityName,
                EntityVersion = state.Version + 1,
                TimeStamp = timeStamp,
                IsDeleted = true,
                RemaningWork = 0u,
                OldRemaningWork = state.RemaningWork,
                ProjectId = None
            };

            return state.ApplyEvent(@event);
        }

        public static Validation<EventAndState> ChangeRemaningWork(
            this TaskEntityState state,
            ChangeRemaningWork cmd)
        {
            var @event = new TaskRemaningWorkChanged()
            {
                EntityId = cmd.EntityId,
                EntityName = _entityName,
                EntityVersion = state.Version + 1,
                TimeStamp = cmd.TimeStamp,
                RemaningWork = cmd.RemaningWork,
                OldRemaningWork = state.RemaningWork,
                ProjectId = state.ProjectId
            };

            return state.WithCheckNotDeleted().Bind(s => s.ApplyEvent(@event));
        }

        public static Validation<EventAndState> RemoveFromProject(
            this TaskEntityState state,
            DateTime timeStamp,
            Guid projectId)
        {
            var @event = new TaskRemovedFromProject()
            {
                EntityId = state.TaskId,
                EntityName = _entityName,
                EntityVersion = state.Version + 1,
                TimeStamp = timeStamp,
                ProjectId = None,
                OldProjectId = state.ProjectId,
                RemaningWork = state.RemaningWork
            };

            return state
               .WithCheckNotDeleted()
               .Bind(s => s.WithCheckProjectIsLinked(projectId))
               .Bind(s => s.ApplyEvent(@event));
        }

        public static Validation<EventAndState> LinkToProject(
            this TaskEntityState state, 
            DateTime timeStamp,
            Guid projectId)
        {
            var @event = new TaskLinkedToProject()
            {
                EntityId = state.TaskId,
                EntityName = _entityName,
                EntityVersion = state.Version + 1,
                TimeStamp = timeStamp,
                ProjectId = projectId,
                RemaningWork = state.RemaningWork
            };

            return state
                .WithCheckNotDeleted()
                .Bind(s => s.WithCheckProjectNotAlreadyLinked(projectId))
                .Bind(s => s.ApplyEvent(@event));
        }

        private static Validation<TaskEntityState> WithCheckNotDeleted(
                this TaskEntityState state) =>
            state.IsDeleted
               ? Invalid("Impossible de modifier une tâche supprimée")
               : state;

        private static Validation<TaskEntityState> WithCheckProjectNotAlreadyLinked(
                this TaskEntityState state, Guid projectId) =>
            state.ProjectId.Match(
                None: () => state,
                Some: (id) => id.Equals(projectId)
                    ? Invalid("La tâche est déjà associée au projet")
                    : Valid(state));

        private static Validation<TaskEntityState> WithCheckProjectIsLinked(
                this TaskEntityState state, Guid projectId) =>
            state.ProjectId == projectId
                ? Valid(state)
                : Invalid("La tâche nest pas associée au projet");
    }
}
