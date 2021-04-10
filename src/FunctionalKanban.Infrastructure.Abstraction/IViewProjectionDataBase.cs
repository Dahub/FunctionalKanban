namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using Unit = System.ValueTuple;

    public interface IViewProjectionDataBase
    {
        IEnumerable<TaskViewProjection> TaskViewProjections { get; }

        Exceptional<IEnumerable<T>> Projections<T>() where T : ViewProjection;

        Exceptional<Unit> Upsert<T>(T viewProjection) where T : ViewProjection;
    }
}
