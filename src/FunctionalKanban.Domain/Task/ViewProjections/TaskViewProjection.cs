namespace FunctionalKanban.Domain.Task.ViewProjections
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Functional;
    using Unit = System.ValueTuple;

    public record TaskViewProjection : ViewProjection
    {
        public string Name { get; init; }

        public uint RemaningWork { get; init; }

        public TaskStatus Status { get; init; }
    }

    public static class TaskViewProjectionExt
    {
        public static TaskViewProjection With(this TaskViewProjection view, Event @event) =>
            @event switch
            {
                TaskCreated e => view with { Id = e.AggregateId, Name = e.Name, RemaningWork = e.RemaningWork, Status = e.Status },
                TaskStatusChanged e => view with { RemaningWork = e.RemaningWork, Status = e.NewStatus },
                _ => view with { }
            };
    }

    public class TaskViewProjectionHandler : ViewProjectionHandler<TaskViewProjection>
    {
        public TaskViewProjectionHandler(
            Func<Guid, Option<TaskViewProjection>> getViewProjectionById,
            Func<TaskViewProjection, Exceptional<Unit>> upsertViewProjection) :
            base(getViewProjectionById, upsertViewProjection)
        { }

        protected override bool CanHandle(Event @event) =>
            @event is TaskCreated or TaskStatusChanged;

        protected override Exceptional<TaskViewProjection> UpdateViewProjection(
                Func<Guid, Option<TaskViewProjection>> getViewProjectionById,
                Event @event) =>
            @event switch
            {
                TaskCreated e => new TaskViewProjection().With(e),
                _ => getViewProjectionById(@event.AggregateId).Match(
                        None: () => BuildError(),
                        Some: (p) => p.With(@event))
            };
    }
}
