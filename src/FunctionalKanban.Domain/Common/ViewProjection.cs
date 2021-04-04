namespace FunctionalKanban.Domain.Common
{
    using System;

    public abstract record ViewProjection
    {
        public Guid Id { get; init; }
    }
}
