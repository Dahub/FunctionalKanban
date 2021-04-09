namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Task.ViewProjections;

    public interface IViewProjectionDataBase
    {
        IEnumerable<TaskViewProjection> TaskViewProjections { get; }

        void Upsert(TaskViewProjection viewProjection);
    }
}
