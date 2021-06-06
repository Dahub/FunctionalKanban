namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System;
    using System.Linq;
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;
    using Unit = System.ValueTuple;

    public interface IViewProjectionDataBase
    {
        Exceptional<IQueryable<T>> Projections<T>() where T : ViewProjection;

        Exceptional<IQueryable<ViewProjection>> Projections(Type type, Func<ViewProjection, bool> predicate);

        Exceptional<Unit> Upsert<T>(T viewProjection) where T : ViewProjection;

        Exceptional<Unit> Delete<T>(T viewProjection) where T : ViewProjection;
    }
}
