namespace FunctionalKanban.Domain.ViewProjections
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Events;
    using LaYumba.Functional;

    public record DeletedTaskViewProjection : ViewProjection
    {
        public DateTime DeletedAt { get; init; }

        public override Option<ViewProjection> With(Event @event) =>
            @event switch
            {
                TaskDeleted e   => this with { Id = e.EntityId, DeletedAt = e.TimeStamp },
                _               => this with { }
            };
    }
}
