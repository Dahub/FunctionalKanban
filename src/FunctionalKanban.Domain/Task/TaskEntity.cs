namespace FunctionalKanban.Domain.Task
{
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
                EntityId = cmd.EntityId,
                Name = cmd.Name,
                RemaningWork = cmd.RemaningWork,
                TimeStamp = cmd.TimeStamp,
                Status = TaskStatus.Todo,
                EntityVersion = 1
            };

            var state = new TaskState()
            {
                Version = 1,
                RemaningWork = @event.RemaningWork,
                TaskName = @event.Name,
                TaskStatus = @event.Status
            };

            return new EventAndState(@event, state);
        }
       

        public static Validation<EventAndState> ChangeStatus(this TaskState state, ChangeTaskStatus cmd)
        {
            var @event = new TaskStatusChanged() 
            { 
                EntityId = cmd.EntityId, 
                EntityVersion = state.Version + 1, 
                NewStatus = cmd.TaskStatus,
                TimeStamp = cmd.TimeStamp
            };

            return state
                .ApplyEvent(@event)
                .Bind<TaskState, EventAndState>((s) => new EventAndState(@event, s));
        }

        private static Validation<TaskState> ApplyEvent(this TaskState state, Event @event) =>
            (@event) switch
            {
                TaskStatusChanged e => state with { TaskStatus = e.NewStatus },
                _ => Invalid(Error("Type d'événement non pris en charge"))
            };
    }
}
