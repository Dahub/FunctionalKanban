namespace FunctionalKanban.Domain.ViewProjections
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Events;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    public record DeletedTaskViewProjection : ViewProjection
    {
        public DateTime DeletedAt { get; init; }

        public static Option<Guid> HandleWithId(Event @event) =>
            @event switch
            {
                TaskDeleted e => e.EntityId,
                _ => None
            };

        public override Option<ViewProjection> With(Event @event) =>
            @event switch
            {
                TaskDeleted e   => this with { Id = e.EntityId, DeletedAt = e.TimeStamp },
                _               => this with { }
            };
    }
}
