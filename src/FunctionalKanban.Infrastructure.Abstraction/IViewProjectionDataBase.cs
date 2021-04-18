namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.ViewProjections;
    using LaYumba.Functional;
    using Unit = System.ValueTuple;

    public interface IViewProjectionDataBase
    {
        IEnumerable<TaskViewProjection> TaskViewProjections { get; }

        Exceptional<IEnumerable<T>> Projections<T>() where T : ViewProjection;

        Exceptional<IEnumerable<ViewProjection>> Projections(Type type);

        Exceptional<Unit> Upsert<T>(T viewProjection) where T : ViewProjection;

        Exceptional<Unit> Delete<T>(Guid id) where T : ViewProjection;
    }
}
