namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using Unit = System.ValueTuple;

    public interface IViewProjectionRepository
    {
        Exceptional<Option<T>> GetById<T>(Guid id) where T : ViewProjection;

        Exceptional<Unit> Upsert<T>(T viewProjection) where T : ViewProjection;

        Exceptional<IEnumerable<T>> Get<T>(Func<T, bool> predicate) where T : ViewProjection;

        Exceptional<IEnumerable<ViewProjection>> GetWithType(Type type, Func<ViewProjection, bool> predicate);
    }
}
