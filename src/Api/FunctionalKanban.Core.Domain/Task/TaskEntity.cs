namespace FunctionalKanban.Core.Domain.Task
{
    using System;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.Task.Commands;
    using FunctionalKanban.Core.Domain.Task.Events;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    public static class TaskEntity
    {
        public static Validation<EventAndState> Create(CreateTask cmd)
        {
            var @event = new TaskCreated()
            {
                EntityId        = cmd.EntityId,
                Name            = cmd.Name,
                RemaningWork    = cmd.RemaningWork,
                IsDeleted       = false,
                TimeStamp       = cmd.TimeStamp,
                Status          = TaskStatus.Todo,
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
                EntityId        = cmd.EntityId,
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
                DeleteTask cmd) => 
            state.WithCheckNotDeleted().Bind(s => state.Delete(cmd.TimeStamp));

        public static Validation<EventAndState> Delete(
                this TaskEntityState state,
                DateTime timeStamp)
        {
            var @event = new TaskDeleted()
            {
                EntityId = state.TaskId,
                TimeStamp = timeStamp,
                IsDeleted = true,
                RemaningWork = 0u,
                OldRemaningWork = state.RemaningWork,
                ProjectId = None
            };

            return state.WithCheckNotDeleted().Bind(s => s.ApplyEvent(@event));
        }

        public static Validation<EventAndState> ChangeRemaningWork(
            this TaskEntityState state,
            ChangeRemaningWork cmd)
        {
            var @event = new TaskRemaningWorkChanged()
            {
                EntityId = cmd.EntityId,
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
                : Invalid("La tâche n'est pas associée au projet");
    }
}
