﻿namespace FunctionalKanban.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using FunctionalKanban.Infrastructure.InMemory;
    using static FunctionalKanban.Functional.F;
    using Unit = System.ValueTuple;

    public class ViewProjectionRepository<T> : IViewProjectionRepository<T> where T : ViewProjection
    {
        private readonly IViewProjectionDataBase _dataBase;

        public ViewProjectionRepository(IViewProjectionDataBase dataBase) => _dataBase = dataBase;

        public Exceptional<IEnumerable<ViewProjection>> Get(Func<ViewProjection, bool> predicate) =>
            Try(() =>
            {
                if (typeof(T) == typeof(TaskViewProjection))
                {
                    return GetByPredicate(predicate, _dataBase.TaskViewProjections.Map(p => p as T));
                }

                throw new Exception($"projection de type {typeof(T)} non prise en charge");
            }).Run();

        public Exceptional<Option<T>> GetById(Guid id) =>
            Try(() =>
            {
                if (typeof(T) == typeof(TaskViewProjection))
                {
                    return GetTaskViewProjectionById(id).Map((p) => p as T);
                }

                return None;
            }).Run();

        public Exceptional<Unit> Upsert(T viewProjection) =>
            Try(() =>
            {
                if (typeof(T) == typeof(TaskViewProjection))
                {
                    _dataBase.Upsert(viewProjection as TaskViewProjection);

                    return Unit.Create();
                }

                throw new Exception($"Impossible d'insérer le type de projection {typeof(T)}");
            }).Run();

        private static IEnumerable<ViewProjection> GetByPredicate(Func<ViewProjection, bool> predicate, IEnumerable<T> projections) =>
            projections.Where(predicate);

        private Option<TaskViewProjection> GetTaskViewProjectionById(Guid id)
        {
            var projection = _dataBase.TaskViewProjections.FirstOrDefault(p => p.Id.Equals(id));
            return projection == null ? None : Some(projection);
        }
    }
}