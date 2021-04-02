namespace FunctionalKanban.Domain.Task
{
    using System.Collections.Generic;
    using System.Linq;
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
                AggregateId =   cmd.AggregateId,
                AggregateName = typeof(TaskEntity).FullName,
                Name =          cmd.Name,
                RemaningWork =  cmd.RemaningWork,
                TimeStamp =     cmd.TimeStamp,
                Status =        TaskStatus.Todo,
                EntityVersion = 1
            };

            return ApplyEvent(new TaskState(), @event).ToEventAndState(@event);
        }          

        public static Validation<EventAndState> ChangeStatus(
                this TaskState state, 
                ChangeTaskStatus cmd)
        {
            var @event = new TaskStatusChanged() 
            { 
                AggregateId =   cmd.AggregateId,
                AggregateName = typeof(TaskEntity).FullName,
                EntityVersion = state.Version + 1, 
                NewStatus =     cmd.TaskStatus,
                TimeStamp =     cmd.TimeStamp
            };

            return ApplyEvent(state, @event).ToEventAndState(@event);
        }

        public static Option<TaskState> From(IEnumerable<Event> history) =>
            history.OrderBy(h => h.EntityVersion).Match(
                   Empty: () => None,
                   Otherwise: (createdEvent, otherEvents) =>
                      otherEvents.Aggregate(
                         seed: ApplyEvent(new TaskState(), (TaskCreated)createdEvent),
                         func: (state, evt) => state.Bind(s => ApplyEvent(s, evt))).Match(
                            Invalid: (_) => None,
                            Valid: (state) => Some(state)));

        private static Validation<TaskState> ApplyEvent(
                TaskState state,
                Event @event) =>
            (@event) switch
            {
                TaskCreated e       => state with { Version = 1, RemaningWork = e.RemaningWork, TaskName = e.Name, TaskStatus = e.Status },
                TaskStatusChanged e => state with { TaskStatus = e.NewStatus },
                _                   => Invalid(Error("Type d'événement non pris en charge"))
            };

        private static Validation<EventAndState> ToEventAndState(this Validation<TaskState> state, Event @event) => 
            state.Bind<TaskState, EventAndState>((s) => new EventAndState(@event, s));
    }
}
