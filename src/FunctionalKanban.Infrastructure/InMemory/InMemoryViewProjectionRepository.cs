namespace FunctionalKanban.Infrastructure.InMemory
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

    public class InMemoryViewProjectionRepository<T> : IViewProjectionRepository<T> where T : ViewProjection
    {
        private readonly IInMemoryDatabase _inMemoryDataBase;

        public InMemoryViewProjectionRepository(IInMemoryDatabase inMemoryDatabase) => _inMemoryDataBase = inMemoryDatabase;

        public Exceptional<IEnumerable<T>> Get(Func<T, bool> predicate)
        {
            if (typeof(T) == typeof(TaskViewProjection))
            {
                return GetByPredicate(predicate, _inMemoryDataBase.TaskViewProjections.Map(p => p as T));
            }

            return new Exception($"projection de type {typeof(T)} non prise en charge");
        }
        
        public Exceptional<Option<T>> GetById(Guid id)
        {
            if (typeof(T) == typeof(TaskViewProjection))
            {
                return GetTaskViewProjectionById(id).Map((p) => p as T);
            }

            return Exceptional<Option<T>>(None);
        }

        public Exceptional<Unit> Upsert(T viewProjection)
        {
            if (typeof(T) == typeof(TaskViewProjection))
            {
                _inMemoryDataBase.Upsert(viewProjection as TaskViewProjection);

                return Unit.Create();
            }

            return new Exception($"Impossible d'insérer le type de projection {typeof(T)}");
        }
        private Exceptional<IEnumerable<T>> GetByPredicate(Func<T, bool> predicate, IEnumerable<T> projections) =>
            Exceptional(projections.Where(predicate));

        private Option<TaskViewProjection> GetTaskViewProjectionById(Guid id)
        {
            var projection = _inMemoryDataBase.TaskViewProjections.FirstOrDefault(p => p.Id.Equals(id));
            return projection == null ? None : Some(projection);
        }
    }
}
