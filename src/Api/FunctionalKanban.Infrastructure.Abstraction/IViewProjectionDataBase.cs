namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;
    using Unit = System.ValueTuple;

    public interface IViewProjectionDataBase
    {
        Exceptional<IEnumerable<T>> Projections<T>() where T : ViewProjection;

        Exceptional<IEnumerable<ViewProjection>> Projections(Type type, Expression<Func<ViewProjection, bool>> predicate);

        Exceptional<Unit> Upsert<T>(T viewProjection) where T : ViewProjection;

        Exceptional<Unit> Delete<T>(T viewProjection) where T : ViewProjection;
    }
}
