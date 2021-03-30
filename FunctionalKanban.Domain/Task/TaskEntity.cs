namespace FunctionalKanban.Domain.Task
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    public static class TaskEntity
    {
        public static TaskState Create(TaskCreated @event) => new TaskState()
        {
            Version = 1,
            RemaningWork = @event.RemanigWork,
            TaskName = @event.Name,
            TaskStatus = @event.Status
        };

        public static Validation<(Event @event, TaskState state)> ChangeStatus(this TaskState state, ChangeTaskStatus cmd)
        {
            var @event = new TaskStatusChanged() 
            { 
                EntityId = cmd.TaskId, 
                EntityVersion = state.Version + 1, 
                NewStatus = cmd.TaskStatus,
                TimeStamp = cmd.TimeStamp
            };

            return state
                .ApplyEvent(@event)
                .Bind<TaskState, (Event, TaskState)>((s) => (@event, s));
        }

        private static Validation<TaskState> ApplyEvent(this TaskState state, Event @event) =>
            (@event) switch
            {
                TaskStatusChanged e => state with { TaskStatus = e.NewStatus },
                _ => Invalid(Error("Type d'événement non pris en charge"))
            };
    }
}
