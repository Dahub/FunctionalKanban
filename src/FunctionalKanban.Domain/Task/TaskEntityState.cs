namespace FunctionalKanban.Domain.Task
{
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Functional;

    public sealed record TaskEntityState : State
    {
        public string TaskName { get; init; }

        public TaskStatus TaskStatus { get; init; }

        public uint RemaningWork { get; init; }

        public override Option<State> From(IEnumerable<Event> history) => 
            From<TaskCreated>(history, () => new TaskEntityState());
       
        protected override State With(Event @event) =>
            @event switch
            {
                TaskCreated e       => this with { Version = e.EntityVersion, RemaningWork = e.RemaningWork, TaskName = e.Name, TaskStatus = e.Status },
                TaskStatusChanged e => this with { Version = e.EntityVersion, TaskStatus = e.NewStatus, RemaningWork = e.RemaningWork },
                _                   => this with { }
            };
    }
}
