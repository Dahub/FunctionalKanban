namespace FunctionalKanban.Domain.Task
{
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    public static class TaskEntity
    {
        public static Validation<EventAndState> Create(CreateTask cmd)
        {
            var @event = new TaskCreated()
            {
                AggregateId = cmd.AggregateId,
                AggregateName = typeof(TaskEntity).FullName,
                Name = cmd.Name,
                RemaningWork = cmd.RemaningWork,
                TimeStamp = cmd.TimeStamp,
                Status = TaskStatus.Todo,
                EntityVersion = 1
            };

            return new TaskState().ApplyEvent(@event).ToEventAndState(@event);
        }

        public static Validation<EventAndState> ChangeStatus(
                this TaskState state,
                ChangeTaskStatus cmd)
        {
            var @event = new TaskStatusChanged()
            {
                AggregateId = cmd.AggregateId,
                AggregateName = typeof(TaskEntity).FullName,
                EntityVersion = state.Version + 1,
                NewStatus = cmd.TaskStatus,
                TimeStamp = cmd.TimeStamp,
                RemaningWork = cmd.TaskStatus.Equals(TaskStatus.Done | TaskStatus.Canceled) ? 0 : state.RemaningWork
            };

            return state.ApplyEvent(@event).ToEventAndState(@event);
        }

        public static Option<TaskState> From(IEnumerable<Event> history) =>
            EntityHelper.From<TaskState, TaskCreated>(history, () => new TaskState(), (state, evt) => ApplyEvent(state, evt));

        private static Validation<TaskState> ApplyEvent(this TaskState state, Event @event) =>
            @event switch
            {
                TaskCreated e => state with { Version = e.EntityVersion, RemaningWork = e.RemaningWork, TaskName = e.Name, TaskStatus = e.Status },
                TaskStatusChanged e => state with { Version = e.EntityVersion, TaskStatus = e.NewStatus, RemaningWork = e.RemaningWork },
                _ => Invalid(Error("Type d'événement non pris en charge"))
            };
    }
}
