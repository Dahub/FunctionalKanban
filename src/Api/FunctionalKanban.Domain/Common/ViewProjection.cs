namespace FunctionalKanban.Domain.Common
{
    using System;
    using LaYumba.Functional;

    public abstract record ViewProjection
    {
        public Guid Id { get; init; }

        public abstract Option<ViewProjection> With(Event @event);
    }
}
