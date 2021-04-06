namespace FunctionalKanban.Domain.Task.ViewProjections
{
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Events;

    public record TaskViewProjection : ViewProjection
    {
        public string Name { get; init; }

        public uint RemaningWork { get; init; }

        public TaskStatus Status { get; init; }

        public static bool CanHandle(Event @event) => @event is TaskCreated or TaskStatusChanged;
    }

    public static class TaskViewProjectionExt
    {
        public static TaskViewProjection With(this TaskViewProjection view, Event @event) =>
            @event switch
            {
                TaskCreated e       => view with { Id = e.AggregateId, Name = e.Name, RemaningWork = e.RemaningWork, Status = e.Status },
                TaskStatusChanged e => view with { RemaningWork = e.RemaningWork, Status = e.NewStatus },
                _                   => view with { }
            };
    }
}
