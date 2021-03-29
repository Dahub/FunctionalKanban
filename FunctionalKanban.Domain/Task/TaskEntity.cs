namespace FunctionalKanban.Domain.Task
{
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Events;

    public static class TaskEntity
    {
        public static TaskState Create(TaskCreated @event) => new TaskState(@event.Name, @event.Status);

        public static TaskState Apply(this TaskState state, Event @event) =>
            (@event) switch
            {
                TaskStatusChanged e => state with { TaskStatus = e.NewStatus },
                _ => throw new System.NotImplementedException()
            };
    }
}
