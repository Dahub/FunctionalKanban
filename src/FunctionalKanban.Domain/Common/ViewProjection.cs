namespace FunctionalKanban.Domain.Common
{
    using System;

    public abstract record ViewProjection
    {
        public Guid Id { get; init; }

        public abstract ViewProjection With(Event @event);
    }
}
