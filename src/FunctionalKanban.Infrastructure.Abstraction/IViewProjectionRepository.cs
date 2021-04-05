namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using Unit = System.ValueTuple;

    public interface IViewProjectionRepository<T> where T: ViewProjection
    {
        Exceptional<Option<T>> GetById(Guid id);

        Exceptional<Unit> Upsert(T viewProjection);

        Try<IEnumerable<ViewProjection>> Get(Func<ViewProjection, bool> predicate);
    }
}
