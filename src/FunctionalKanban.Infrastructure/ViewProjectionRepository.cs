namespace FunctionalKanban.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using static FunctionalKanban.Functional.F;
    using Unit = System.ValueTuple;

    public class ViewProjectionRepository<T> : IViewProjectionRepository<T> where T : ViewProjection
    {
        private readonly IViewProjectionDataBase _dataBase;

        public ViewProjectionRepository(IViewProjectionDataBase dataBase) => _dataBase = dataBase;

        public Exceptional<IEnumerable<T>> Get(Func<T, bool> predicate) =>
            _dataBase.Projections<T>()
            .Bind(ps => GetByPredicate(predicate, ps));
        
        public Exceptional<Option<T>> GetById(Guid id) =>
            Try(() =>
            {
                if (typeof(T) == typeof(TaskViewProjection))
                {
                    return GetTaskViewProjectionById(id).Map((p) => p as T);
                }

                return None;
            }).Run();

        public Exceptional<Unit> Upsert(T viewProjection) => _dataBase.Upsert(viewProjection);

        private static Exceptional<IEnumerable<T>> GetByPredicate(Func<T, bool> predicate, IEnumerable<T> ps) =>
            Try(() => ps.Map(p => (T)p).Where(predicate)).Run();

        private Option<TaskViewProjection> GetTaskViewProjectionById(Guid id)
        {
            var projection = _dataBase.TaskViewProjections.FirstOrDefault(p => p.Id.Equals(id));
            return projection == null ? None : Some(projection);
        }
    }
}
