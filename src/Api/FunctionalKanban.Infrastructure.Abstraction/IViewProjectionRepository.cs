namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;
    using Unit = System.ValueTuple;

    public interface IViewProjectionRepository
    {
        Exceptional<Option<T>> GetById<T>(Guid id) where T : ViewProjection;

        Exceptional<Unit> Upsert<T>(T viewProjection) where T : ViewProjection;

        Exceptional<Unit> Delete<T>(T viewProjection) where T : ViewProjection;

        Exceptional<IEnumerable<ViewProjection>> Get(Type projectionType, Func<ViewProjection, bool> predicate);
    }
}
