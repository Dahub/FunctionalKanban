namespace FunctionalKanban.Domain.Task
{
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Events;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    public static class TaskEntity
    {
        private static readonly string _entityName = typeof(TaskEntityState).FullName??string.Empty;

        public static Validation<EventAndState> Create(CreateTask cmd)
        {
            var @event = new TaskCreated()
            {
                EntityId     = cmd.EntityId,
                EntityName   = _entityName,
                Name            = cmd.Name,
                RemaningWork    = cmd.RemaningWork,
                IsDeleted       = false,
                TimeStamp       = cmd.TimeStamp,
                Status          = TaskStatus.Todo,
                EntityVersion   = 1,
                ProjectId       = cmd.ProjectId.HasValue?Some(cmd.ProjectId.Value):None
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
                DeleteTask cmd)
        {
            var @event = new TaskDeleted()
            {
                EntityId = cmd.EntityId,
                EntityName = _entityName,
                EntityVersion = state.Version + 1,
                TimeStamp = cmd.TimeStamp,
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
                OldRemaningWork = state.RemaningWork
            };

            return state.WithCheckNotDeleted().Bind(s => s.ApplyEvent(@event));
        }

        public static Validation<EventAndState> LinkToProject(
            this TaskEntityState state,
            LinkToProject cmd)
        {
            var @event = new TaskLinkedToProject()
            {
                EntityId = cmd.EntityId,
                EntityName = _entityName,
                EntityVersion = state.Version + 1,
                TimeStamp = cmd.TimeStamp,
                ProjectId = cmd.ProjectId == default?None:cmd.ProjectId
            };

            return state.WithCheckNotDeleted().Bind(s => s.ApplyEvent(@event));
        }

        private static Validation<TaskEntityState> WithCheckNotDeleted(
                this TaskEntityState state) =>
            state.IsDeleted
               ? Invalid("Impossible de modifier une tâche supprimée")
               : state;
    }
}
